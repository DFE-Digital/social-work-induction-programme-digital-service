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
    #rsync -av --ignore-existing /var/www/moodledata_ref/ /var/www/moodledata/
    #log "Initially copying reference Moodle data to moodledata..."
    # if [ -z "$(ls -A '/var/www/moodledata_share')" ]; then
    #     log "Azure file share is empty, seeding with moodledata reference data..."
    #     rsync -av /var/www/moodledata_ref/ /var/www/moodledata_share/
    # else
    #     log "Azure file share is NOT empty, syncing to local Moodle data..."
    #     rsync --chown=www-data:www-data -av --ignore-existing /var/www/moodledata_share/ /var/www/moodledata/
    # fi
    # if [[ "$MOODLE_PERSISTED_FILE_SYNC" == 'true' ]]; then
    #     log "Starting background persisted file sync process..."
    #     (
    #         while true; do
    #             rsync -a --delete /var/www/moodledata/ /var/www/moodledata_share/ || log "Background sync failed"
    #             sleep 30
    #         done
    #     ) &        
    # fi
fi

cd /var/www/html/public

# Note, we run moosh with the '-n' option below to switch off permission checking on the
# moodledata directory. This is because currently we haven't been able to configure
# mount options for the Azure Files share attached to the directory. Ideally, we'd like something
# like: -o "...,uid=33,gid=33,file_mode=0770,dir_mode=0770" but this hasn't worked through 
# Terraform, ARM or even locally (attempting to use mount directly). This would assign ownership
# to www-data, as opposed to allowing everyone access.

if [[ "$MOODLE_SWITCH_OFF_OAUTH" == 'true' ]]; then
    log "Switching off OAuth..."
    log "Disabling oidc authentication method in Moodle config..."
    su -s /bin/sh www-data -c 'moosh -n auth-manage disable oidc'
    log "Enabling email-based self-registration..."
    su -s /bin/sh www-data -c 'moosh -n auth-manage enable email'
else
    log "Switching on OAuth..."
    log "Enabling oidc authentication method in Moodle config..."
    su -s /bin/sh www-data -c 'moosh -n auth-manage enable oidc'
    log "Disabling email-based self-registration..."
    su -s /bin/sh www-data -c 'moosh -n auth-manage disable email'
fi
su -s /bin/sh www-data -c 'php admin/cli/purge_caches.php'

# exec so Apache gets PID 1 and handles signals cleanly - will be serving as user www-data
log "Starting Apache..."
exec apache2-foreground
