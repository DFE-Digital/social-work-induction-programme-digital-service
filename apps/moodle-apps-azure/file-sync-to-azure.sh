#!/bin/bash
set -euo pipefail

# Configuration
MOODLE_PATH="/var/www/html/public"
INVENTORY_FILE="/tmp/moodle-inventory.json"
BLOB_CONTENT_PATH="content"  # Single folder structure in blob
LOCK_FILE="/tmp/sync-changes.lock"
LOCK_TIMEOUT=1800  # 30 minutes
INSTANCE_ID="${WEBSITE_INSTANCE_ID:-$(hostname)}"

# Azure Storage configuration (from app settings)
export STORAGE_ACCOUNT_NAME="${STORAGE_ACCOUNT_NAME}"
export STORAGE_CONTAINER_CONFIG="${STORAGE_CONTAINER_CONFIG}"

log() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] [SYNC] [$INSTANCE_ID] $1"
}

acquire_lock() {
    local current_time=$(date +%s)
    
    # Check if lock file exists
    if [[ -f "$LOCK_FILE" ]]; then
        local lock_content=$(cat "$LOCK_FILE" 2>/dev/null || echo "")
        if [[ -n "$lock_content" ]]; then
            local lock_time=$(echo "$lock_content" | cut -d: -f2)
            local lock_pid=$(echo "$lock_content" | cut -d: -f3)
            local time_diff=$((current_time - lock_time))
            
            # Check if lock is stale or process no longer exists
            if [[ $time_diff -lt $LOCK_TIMEOUT ]] && kill -0 "$lock_pid" 2>/dev/null; then
                log "Sync already in progress (PID: $lock_pid), skipping"
                return 1
            else
                log "Stale lock detected, removing"
                rm -f "$LOCK_FILE"
            fi
        fi
    fi
    
    # Create lock
    echo "$INSTANCE_ID:$current_time:$$" > "$LOCK_FILE"
    log "Lock acquired (PID: $$)"
    return 0
}

release_lock() {
    rm -f "$LOCK_FILE"
    log "Lock released"
}

detect_changes() {
    local current_inventory="/tmp/current-inventory-$$.json"
    local changes_file="/tmp/changes-$$.json"
    
    log "Detecting changes..."
    
    # Generate current inventory
    /app/file-sync-generate-inventory.sh generate > /dev/null
    if [[ ! -f "$INVENTORY_FILE" ]]; then
        log "Failed to generate current inventory"
        return 1
    fi
    
    # If no previous inventory exists, treat everything as new
    if [[ ! -f "$INVENTORY_FILE.previous" ]]; then
        log "No previous inventory found, treating all files as new"
        jq '.files | keys[]' "$INVENTORY_FILE" | sed 's/"//g' > "/tmp/changed-files-$$"
        return 0
    fi
    
    # Compare inventories using jq
    cat > "/tmp/compare-script-$$.jq" << 'EOF'
def compare_files:
    . as $current |
    $previous.files as $prev_files |
    $current.files | to_entries | map(
        select(
            .key as $path |
            ($prev_files[$path] // null) as $prev |
            $prev == null or
            .value.size != $prev.size or
            .value.mtime != $prev.mtime or
            .value.checksum != $prev.checksum
        ) | .key
    );

compare_files
EOF
    
    # Find changed files
    jq --argfile previous "$INVENTORY_FILE.previous" \
       -f "/tmp/compare-script-$$.jq" \
       "$INVENTORY_FILE" | \
       jq -r '.[]' > "/tmp/changed-files-$$"
    
    # Also find deleted files (in previous but not in current)
    jq -r '.files | keys[]' "$INVENTORY_FILE.previous" | sort > "/tmp/prev-files-$$"
    jq -r '.files | keys[]' "$INVENTORY_FILE" | sort > "/tmp/curr-files-$$"
    comm -23 "/tmp/prev-files-$$" "/tmp/curr-files-$$" > "/tmp/deleted-files-$$"
    
    # Cleanup temp comparison files
    rm -f "/tmp/compare-script-$$.jq" "/tmp/prev-files-$$" "/tmp/curr-files-$$"
    
    local changed_count=$(wc -l < "/tmp/changed-files-$$")
    local deleted_count=$(wc -l < "/tmp/deleted-files-$$")
    
    log "Changes detected: $changed_count modified/new, $deleted_count deleted"
    
    if [[ $changed_count -eq 0 && $deleted_count -eq 0 ]]; then
        rm -f "/tmp/changed-files-$$" "/tmp/deleted-files-$$"
        return 1  # No changes
    fi
    
    return 0  # Changes detected
}

sync_changes() {
    local changed_files="/tmp/changed-files-$$"
    local deleted_files="/tmp/deleted-files-$$"
    
    if [[ ! -f "$changed_files" && ! -f "$deleted_files" ]]; then
        log "No changes to sync"
        return 0
    fi
    
    local sync_count=0
    
    # Upload changed/new files
    if [[ -f "$changed_files" && -s "$changed_files" ]]; then
        log "Syncing modified/new files..."
        while IFS= read -r relative_path; do
            local source_file="$MOODLE_PATH/$relative_path"
            local blob_path="$BLOB_CONTENT_PATH/$relative_path"
            
            if [[ -f "$source_file" ]]; then
                log "  Uploading: $relative_path"
                
                # Create directory structure in blob if needed
                local blob_dir=$(dirname "$blob_path")
                
                az storage blob upload \
                    --auth-mode login \
                    --account-name "$STORAGE_ACCOUNT_NAME" \
                    --container-name "$STORAGE_CONTAINER_CONFIG" \
                    --name "$blob_path" \
                    --file "$source_file" \
                    --overwrite \
                    --output none
                
                sync_count=$((sync_count + 1))
            else
                log "  Warning: File no longer exists: $source_file"
            fi
        done < "$changed_files"
    fi
    
    # Delete removed files from blob storage
    if [[ -f "$deleted_files" && -s "$deleted_files" ]]; then
        log "Removing deleted files from blob storage..."
        while IFS= read -r relative_path; do
            local blob_path="$BLOB_CONTENT_PATH/$relative_path"
            
            log "  Deleting: $relative_path"
            az storage blob delete \
                --auth-mode login \
                --account-name "$STORAGE_ACCOUNT_NAME" \
                --container-name "$STORAGE_CONTAINER_CONFIG" \
                --name "$blob_path" \
                --output none 2>/dev/null || true
            
            sync_count=$((sync_count + 1))
        done < "$deleted_files"
    fi
    
    log "Sync completed: $sync_count operations performed"
    
    # Cleanup temp files
    rm -f "$changed_files" "$deleted_files"
}

update_inventory() {
    if [[ -f "$INVENTORY_FILE" ]]; then
        cp "$INVENTORY_FILE" "$INVENTORY_FILE.previous"
        log "Inventory updated for next comparison"
    fi
}

main() {
    case "${1:-sync}" in
        "sync")
            if ! acquire_lock; then
                exit 1
            fi
            
            trap release_lock EXIT
            
            if detect_changes; then
                sync_changes
                update_inventory
                log "Sync cycle completed successfully"
            else
                log "No changes detected, skipping sync"
            fi
            ;;
        "force")
            if ! acquire_lock; then
                log "Cannot acquire lock for force sync"
                exit 1
            fi
            
            trap release_lock EXIT
            
            log "Force sync: treating all files as changed"
            rm -f "$INVENTORY_FILE.previous"
            detect_changes  # This will treat everything as new
            sync_changes
            update_inventory
            log "Force sync completed"
            ;;
        "status")
            if [[ -f "$LOCK_FILE" ]]; then
                local lock_content=$(cat "$LOCK_FILE")
                local lock_instance=$(echo "$lock_content" | cut -d: -f1)
                local lock_time=$(echo "$lock_content" | cut -d: -f2)
                local lock_pid=$(echo "$lock_content" | cut -d: -f3)
                
                echo "Sync Status: LOCKED"
                echo "  Instance: $lock_instance"
                echo "  PID: $lock_pid"
                echo "  Locked since: $(date -d @$lock_time)"
                
                if kill -0 "$lock_pid" 2>/dev/null; then
                    echo "  Process: RUNNING"
                else
                    echo "  Process: NOT RUNNING (stale lock)"
                fi
            else
                echo "Sync Status: UNLOCKED"
            fi
            ;;
        *)
            echo "Usage: $0 {sync|force|status}"
            echo "  sync   - Compare with previous state and sync changes"
            echo "  force  - Force sync all files (ignore previous state)"  
            echo "  status - Show current sync lock status"
            exit 1
            ;;
    esac
}

main "$@"