#!/bin/bash

log() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] [STARTUP] $1"
}

azure_login() {
    log "Logging in to Azure with managed identity..."
    az login --identity --allow-no-subscriptions >/dev/null
    if [ $? -ne 0 ]; then
        log "ERROR: Azure login with managed identity failed." >&2
    fi
}

# Support SSH for troubleshooting
log "Starting SSH..."
/usr/sbin/sshd

# The Moodle container will operate in one of 2 modes. Either full-blown Moodle instance, or 
# a cut-down version which just runs the cron jobs

log "Cron configuration, IS_CRON_JOB_ONLY: $IS_CRON_JOB_ONLY"
if [[ "$IS_CRON_JOB_ONLY" == 'true' ]]; then
    log "Switching to cron-only mode..."
    cp /app/apache-config-cron.conf /etc/apache2/sites-available/000-default.conf
    mv /var/www/html/public/version.txt /var/www/html/cron/version.txt

    # It was too involved to get cron to inherit all of the environment variables for
    # Moodle and php, so create a simple background process here. All of the necessary
    # variables are available to the startup process.
    log "Starting periodic background Moodle cron execution job..."
    (
        cd /var/www/html/public
        while true; do
            sleep 60
            su -s /bin/sh www-data -c 'php admin/cli/cron.php'
        done
    ) &        
else 
    # Save non-sensitive environment variables to file (will be used by SSH install / update session)
    /app/save-env.sh /app/env.txt POSTGRES_PASSWORD MOODLE_ADMIN_PASSWORD FILE_STORAGE_ACCESS_KEY

    log "This will be a full Moodle instance..."
    if [[ "$BASIC_AUTH_ENABLED" == 'true' ]]; then
        # Configure basic auth to restrict access / prevent Moodle from being indexed
        log "Configuring basic auth..."
        htpasswd -b -c /etc/apache2/.htpasswd "$BASIC_AUTH_USER" "$BASIC_AUTH_PASSWORD" > /dev/null 2>&1
        cp /app/apache-config-moodle-basic-auth.conf /etc/apache2/sites-available/000-default.conf
    fi
    azure_login
    AZURE_FILE_SHARE="//$FILE_STORAGE_ACCOUNT_NAME.file.core.windows.net/$FILE_STORAGE_SHARE"
    log "Mounting Azure file share: $AZURE_FILE_SHARE for persistent Moodle file storage..."
    mount -t cifs \
        $AZURE_FILE_SHARE \
        /var/www/moodledata \
        -o "vers=3.0,username=$FILE_STORAGE_ACCOUNT_NAME,password=\"$FILE_STORAGE_ACCESS_KEY\",uid=33,gid=33,file_mode=0770,dir_mode=0770,serverino,noperm,rw"
    if [ -z "$(ls -A '/var/www/moodledata')" ]; then
        log "Azure file share is empty, seeding with moodledata reference data..."
        cp -a /var/www/moodledata_ref/. /var/www/moodledata/
        chown -R www-data:www-data /var/www/moodledata/
    else
        log "Azure file share contains files, SKIPPING moodledata reference data seeding..."
        ls -A /var/www/moodledata
    fi
    log "Starting background debug job..."
    (
        # Configuration
        DEBUG_SCRIPT="/app/debug.sh"
        OUTPUT_FILE="/app/debug.txt"
        CHECK_INTERVAL_SECONDS=1

        echo "[$(date -Is)] Debug script watcher started." >> "$OUTPUT_FILE"

        # Loop indefinitely
        while true; do
            if [[ -f "$DEBUG_SCRIPT" ]]; then
                echo -e "\n--- [$(date -Is)] Executing $DEBUG_SCRIPT ---" >> "$OUTPUT_FILE" 2>&1

                # Make executable
                chmod +x "$DEBUG_SCRIPT" >> "$OUTPUT_FILE" 2>&1

                # Execute and pipe all output (stdout and stderr) to the file
                # Use a subshell to capture all output
                ( "$DEBUG_SCRIPT" ) >> "$OUTPUT_FILE" 2>&1

                # Rename the script with a timestamp to indicate it has been processed
                RENAME_TARGET="${DEBUG_SCRIPT}.x"
                mv "$DEBUG_SCRIPT" "$RENAME_TARGET" >> "$OUTPUT_FILE" 2>&1
                echo "--- [$(date -Is)] Renamed to $RENAME_TARGET ---" >> "$OUTPUT_FILE" 2>&1
            fi

            # Wait for the next check
            sleep "$CHECK_INTERVAL_SECONDS"
        done        
    ) &
fi

cd /var/www/html/public
if [[ "$MOODLE_SWITCH_OFF_OAUTH" == 'true' ]]; then
    log "Switching off OAuth..."
    log "Disabling oidc authentication method in Moodle config..."
    su -s /bin/sh www-data -c 'moosh auth-manage disable oidc'
    log "Enabling email-based self-registration..."
    su -s /bin/sh www-data -c 'moosh auth-manage enable email'
else
    log "Switching on OAuth..."
    log "Enabling oidc authentication method in Moodle config..."
    su -s /bin/sh www-data -c 'moosh auth-manage enable oidc'
    log "Disabling email-based self-registration..."
    su -s /bin/sh www-data -c 'moosh auth-manage disable email'
fi
su -s /bin/sh www-data -c 'php admin/cli/purge_caches.php'

# exec so Apache gets PID 1 and handles signals cleanly - will be serving as user www-data
log "Starting Apache..."
exec apache2-foreground
