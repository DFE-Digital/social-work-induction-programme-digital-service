#!/bin/bash
set -euo pipefail

MOODLE_PATH="/var/www/html/public"
# Accept optional extension for inventory file name
INVENTORY_FILE="/tmp/moodle-inventory.json${2:-}"
INSTANCE_ID="${WEBSITE_INSTANCE_ID:-$(hostname)}"

log() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] [INVENTORY] [$INSTANCE_ID] $1"
}

generate_inventory() {
    log "Generating inventory for paths: $MOODLE_FILE_SYNC_PATHS"
    
    local temp_inventory="/tmp/inventory-temp-$$.json"
    
    # Start JSON structure
    cat > "$temp_inventory" << EOF
{
    "generated_at": "$(date -Iseconds)",
    "instance_id": "$INSTANCE_ID",
    "base_path": "$MOODLE_PATH",
    "tracked_paths": [$(echo "$MOODLE_FILE_SYNC_PATHS" | sed 's/ /", "/g' | sed 's/^/"/;s/$/"/')],
    "files": {
EOF

    local first_file=true
    local all_files=()
    
    # Collect all files first to avoid subshell issues
    for path in $MOODLE_FILE_SYNC_PATHS; do
        local full_path="$MOODLE_PATH/$path"
        
        if [[ ! -d "$full_path" ]]; then
            log "Warning: Directory does not exist: $full_path"
            continue
        fi
        
        log "Scanning directory: $path"
        
        # Collect files into array
        while IFS= read -r -d '' file; do
            all_files+=("$file")
        done < <(find "$full_path" -type f \
            -not -path "*/cache/*" \
            -not -path "*/temp/*" \
            -not -path "*/tmp/*" \
            -not -name "*.tmp" \
            -not -name "*.temp" \
            -not -name "*.log" \
            -not -name "*.swp" \
            -not -name "*~" \
            -print0)
    done
    # Process collected files
    for file in "${all_files[@]}"; do
        # Get file stats
        local relative_path="${file#$MOODLE_PATH/}"
        local size=$(stat -c%s "$file" 2>/dev/null || echo "0")
        local mtime=$(stat -c%Y "$file" 2>/dev/null || echo "0")
        local checksum=$(md5sum "$file" 2>/dev/null | cut -d' ' -f1 || echo "unknown")
        
        # Add comma if not first file
        if [[ "$first_file" == "false" ]]; then
            echo "," >> "$temp_inventory"
        fi
        first_file=false
        
        # Add file entry - EOF must be on its own line
        cat >> "$temp_inventory" << EOF
        "$relative_path": {
            "size": $size,
            "mtime": $mtime,
            "checksum": "$checksum",
            "full_path": "$file"
        }
EOF
    done

    # Close JSON structure
    cat >> "$temp_inventory" << 'EOF'

    }
}
EOF

    cp "$temp_inventory" "$INVENTORY_FILE"
    rm -f "$temp_inventory"
    local file_count=$(jq '.files | length' "$INVENTORY_FILE")
    log "Inventory generated successfully: $file_count files tracked"
    log "Inventory saved to: $INVENTORY_FILE"
}

main() {
    case "${1:-generate}" in
        "generate")
            generate_inventory
            ;;
        "show")
            if [[ -f "$INVENTORY_FILE" ]]; then
                cat "$INVENTORY_FILE"
            else
                log "No inventory file found at $INVENTORY_FILE"
                exit 1
            fi
            ;;
        "stats")
            if [[ -f "$INVENTORY_FILE" ]]; then
                local file_count=$(jq '.files | length' "$INVENTORY_FILE")
                local generated_at=$(jq -r '.generated_at' "$INVENTORY_FILE")
                local instance_id=$(jq -r '.instance_id' "$INVENTORY_FILE")
                
                log "Inventory Statistics:"
                log "  Files tracked: $file_count"
                log "  Generated at: $generated_at"
                log "  Instance ID: $instance_id"
            else
                log "No inventory file found"
                exit 1
            fi
            ;;
        *)
            echo "Usage: $0 {generate|show|stats}"
            exit 1
            ;;
    esac
}

main "$@"