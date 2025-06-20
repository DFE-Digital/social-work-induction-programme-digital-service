#!/bin/bash

log() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] [STARTUP] $1"
}

azure_login() {
    az login --identity --allow-no-subscriptions >/dev/null
    if [ $? -ne 0 ]; then
        log "ERROR: Config file syncing - Azure login with managed identity failed." >&2
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
    /app/save-env.sh /app/env.txt POSTGRES_PASSWORD MOODLE_ADMIN_PASSWORD

    log "This will be a full Moodle instance..."
    if [[ "$BASIC_AUTH_ENABLED" == 'true' ]]; then
        # Configure basic auth to restrict access / prevent Moodle from being indexed
        log "Configuring basic auth..."
        htpasswd -b -c /etc/apache2/.htpasswd "$BASIC_AUTH_USER" "$BASIC_AUTH_PASSWORD" > /dev/null 2>&1
        cp /app/apache-config-moodle-basic-auth.conf /etc/apache2/sites-available/000-default.conf
    fi
    if [ -z "$(ls -A '/var/www/moodledata')" ]; then
        log "Azure file share is empty, seeding with moodledata reference data..."
        cp -a /var/www/moodledata_ref/. /var/www/moodledata/
    fi
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
