#!/bin/bash
set -euo pipefail

# Configuration
MOODLE_PATH="/var/www/html/public"
INVENTORY_FILE="/tmp/moodle-inventory.json"
BLOB_CONTENT_PATH="content"  # Single folder structure in blob
LOCK_FILE="/tmp/restore-changes.lock"
LOCK_TIMEOUT=1800  # 30 minutes
INSTANCE_ID="${WEBSITE_INSTANCE_ID:-$(hostname)}"
TEMP_RESTORE_DIR="/tmp/moodle-restore-$$"

# Azure Storage configuration (from app settings)
export STORAGE_ACCOUNT_NAME="${STORAGE_ACCOUNT_NAME}"
export STORAGE_CONTAINER_CONFIG="${STORAGE_CONTAINER_CONFIG}"

log() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] [RESTORE] [$INSTANCE_ID] $1"
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
                log "Restore already in progress (PID: $lock_pid), skipping"
                return 1
            else
                log "Stale lock detected, removing"
                rm -f "$LOCK_FILE"
            fi
        fi
    fi
    
    echo "$INSTANCE_ID:$current_time:$$" > "$LOCK_FILE"
    log "Lock acquired (PID: $$)"
    return 0
}

release_lock() {
    rm -f "$LOCK_FILE"
    log "Lock released"
}

cleanup() {
    rm -rf "$TEMP_RESTORE_DIR"
    rm -f "/tmp/blob-list-$$" "/tmp/restore-summary-$$"
}

list_blob_files() {
    local blob_list="/tmp/blob-list-$$"
    
    log "Listing files in blob storage..."
    
    az storage blob list \
        --auth-mode login \
        --account-name "$STORAGE_ACCOUNT_NAME" \
        --container-name "$STORAGE_CONTAINER_CONFIG" \
        --prefix "$BLOB_CONTENT_PATH/" \
        --output tsv \
        --query "[].name" > "$blob_list"
    
    if [[ ! -s "$blob_list" ]]; then
        log "No files found in blob storage under path: $BLOB_CONTENT_PATH/"
        return 1
    fi
    
    local file_count=$(wc -l < "$blob_list")
    log "Found $file_count files in blob storage"
    return 0
}

restore_files() {
    local blob_list="/tmp/blob-list-$$"
    local summary_file="/tmp/restore-summary-$$"
    local restored_count=0
    local skipped_count=0
    local error_count=0
    
    if [[ ! -f "$blob_list" ]]; then
        log "No blob list found"
        return 1
    fi
    
    log "Starting file restoration..."
    
    # Create temporary restore directory
    mkdir -p "$TEMP_RESTORE_DIR"
    
    while IFS= read -r blob_name; do
        # Skip if blob name doesn't start with our content path
        if [[ ! "$blob_name" =~ ^${BLOB_CONTENT_PATH}/ ]]; then
            continue
        fi
        
        # Extract relative path by removing the blob content path prefix
        local relative_path="${blob_name#${BLOB_CONTENT_PATH}/}"
        local target_file="$MOODLE_PATH/$relative_path"
        local temp_file="$TEMP_RESTORE_DIR/$relative_path"
        
        # Create directory structure in temp location
        local temp_dir=$(dirname "$temp_file")
        mkdir -p "$temp_dir"
        
        # Download blob to temporary location
        log "  Downloading: $relative_path"
        
        if az storage blob download \
            --auth-mode login \
            --account-name "$STORAGE_ACCOUNT_NAME" \
            --container-name "$STORAGE_CONTAINER_CONFIG" \
            --name "$blob_name" \
            --file "$temp_file" \
            --output none 2>/dev/null; then
            
            # Create target directory if it doesn't exist
            local target_dir=$(dirname "$target_file")
            mkdir -p "$target_dir"
            
            # Check if we should overwrite (compare modification times if both exist)
            local should_restore=true
            if [[ -f "$target_file" && "${RESTORE_MODE:-overwrite}" == "newer" ]]; then
                local target_mtime=$(stat -c %Y "$target_file" 2>/dev/null || echo 0)
                local temp_mtime=$(stat -c %Y "$temp_file" 2>/dev/null || echo 0)
                
                if [[ $target_mtime -gt $temp_mtime ]]; then
                    log "    Skipping: local file is newer"
                    should_restore=false
                    skipped_count=$((skipped_count + 1))
                fi
            fi
            
            if [[ "$should_restore" == true ]]; then
                # Move file from temp to target location
                if mv "$temp_file" "$target_file"; then
                    # Set appropriate permissions (readable by web server)
                    chmod 644 "$target_file" 2>/dev/null || true
                    
                    echo "RESTORED: $relative_path" >> "$summary_file"
                    restored_count=$((restored_count + 1))
                    log "    Restored: $relative_path"
                else
                    echo "ERROR: Failed to move $relative_path" >> "$summary_file"
                    error_count=$((error_count + 1))
                    log "    Error: Failed to move $relative_path to target location"
                fi
            fi
        else
            echo "ERROR: Failed to download $relative_path" >> "$summary_file"
            error_count=$((error_count + 1))
            log "    Error: Failed to download $relative_path"
        fi
        
    done < "$blob_list"
    
    log "Restoration completed: $restored_count restored, $skipped_count skipped, $error_count errors"
    
    # Show summary if there were errors
    if [[ $error_count -gt 0 && -f "$summary_file" ]]; then
        log "Error summary:"
        grep "ERROR:" "$summary_file" | while read -r line; do
            log "  $line"
        done
    fi
    
    return $error_count
}

verify_restore() {
    local blob_list="/tmp/blob-list-$$"
    local verification_errors=0
    
    log "Verifying restored files..."
    
    while IFS= read -r blob_name; do
        if [[ ! "$blob_name" =~ ^${BLOB_CONTENT_PATH}/ ]]; then
            continue
        fi
        
        local relative_path="${blob_name#${BLOB_CONTENT_PATH}/}"
        local target_file="$MOODLE_PATH/$relative_path"
        
        if [[ ! -f "$target_file" ]]; then
            log "  Warning: File not found after restore: $relative_path"
            verification_errors=$((verification_errors + 1))
        fi
    done < "$blob_list"
    
    if [[ $verification_errors -eq 0 ]]; then
        log "Verification completed: All files restored successfully"
    else
        log "Verification completed with $verification_errors missing files"
    fi
    
    return $verification_errors
}

main() {
    local restore_mode="${2:-overwrite}"
    
    case "${1:-restore}" in
        "restore")
            export RESTORE_MODE="$restore_mode"
            
            if ! acquire_lock; then
                exit 1
            fi
            
            trap 'release_lock; cleanup' EXIT
            
            if list_blob_files; then
                if restore_files; then
                    verify_restore
                    log "Restore cycle completed successfully"
                else
                    log "Restore completed with errors"
                    exit 1
                fi
            else
                log "No files to restore"
            fi
            ;;
        "restore-newer")
            export RESTORE_MODE="newer"
            
            if ! acquire_lock; then
                exit 1
            fi
            
            trap 'release_lock; cleanup' EXIT
            
            log "Restore mode: Only restore files newer than local versions"
            
            if list_blob_files; then
                if restore_files; then
                    verify_restore
                    log "Conditional restore cycle completed successfully"
                else
                    log "Conditional restore completed with errors"
                    exit 1
                fi
            else
                log "No files to restore"
            fi
            ;;
        "list")
            if list_blob_files; then
                local blob_list="/tmp/blob-list-$$"
                log "Files available for restore:"
                while IFS= read -r blob_name; do
                    if [[ "$blob_name" =~ ^${BLOB_CONTENT_PATH}/ ]]; then
                        local relative_path="${blob_name#${BLOB_CONTENT_PATH}/}"
                        echo "  $relative_path"
                    fi
                done < "$blob_list"
                cleanup
            fi
            ;;
        "status")
            if [[ -f "$LOCK_FILE" ]]; then
                local lock_content=$(cat "$LOCK_FILE")
                local lock_instance=$(echo "$lock_content" | cut -d: -f1)
                local lock_time=$(echo "$lock_content" | cut -d: -f2)
                local lock_pid=$(echo "$lock_content" | cut -d: -f3)
                
                echo "Restore Status: LOCKED"
                echo "  Instance: $lock_instance"
                echo "  PID: $lock_pid"
                echo "  Locked since: $(date -d @$lock_time)"
                
                if kill -0 "$lock_pid" 2>/dev/null; then
                    echo "  Process: RUNNING"
                else
                    echo "  Process: NOT RUNNING (stale lock)"
                fi
            else
                echo "Restore Status: UNLOCKED"
            fi
            ;;
        *)
            echo "Usage: $0 {restore|restore-newer|list|status}"
            echo "  restore       - Restore all files from blob storage (overwrite local)"
            echo "  restore-newer - Only restore files that are newer than local versions"
            echo "  list          - List files available for restore"
            echo "  status        - Show current restore lock status"
            echo ""
            echo "Environment variables:"
            echo "  STORAGE_ACCOUNT_NAME     - Azure storage account name"
            echo "  STORAGE_CONTAINER_CONFIG - Azure storage container name"
            exit 1
            ;;
    esac
}

main "$@"