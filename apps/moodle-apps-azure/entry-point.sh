#!/bin/bash

log() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] [STARTUP] $1"
}

# Support SSH for troubleshooting
log "Starting SSH..."
/usr/sbin/sshd

# Save non-sensitive environment variables to file (will be used by SSH install / update session)
/app/save-env.sh /app/env.txt POSTGRES_PASSWORD MOODLE_ADMIN_PASSWORD

# The Moodle container will operate in one of 2 modes. Either full-blown Moodle instance, or 
# a cut-down version which just runs the cron jobs

log "Cron configuration, IS_CRON_JOB_ONLY: $IS_CRON_JOB_ONLY"
if [[ "$IS_CRON_JOB_ONLY" == 'true' ]]; then
    log "Switching to cron-only mode..."
    cp /app/apache-config-cron.conf /etc/apache2/sites-available/000-default.conf
    cp /app/moodle-cron /etc/cron.d/moodle-cron
    chmod 0644 /etc/cron.d/moodle-cron
    mv /var/www/html/public/version.txt /var/www/html/cron/version.txt
    log "Starting cron daemon..."
    /usr/sbin/cron
else 
    log "This will be a full Moodle instance..."
    if [[ "$BASIC_AUTH_ENABLED" == 'true' ]]; then
        # Configure basic auth to restrict access / prevent Moodle from being indexed
        log "Configuring basic auth..."
        htpasswd -b -c /etc/apache2/.htpasswd "$BASIC_AUTH_USER" "$BASIC_AUTH_PASSWORD" > /dev/null 2>&1
        cp /app/apache-config-moodle-basic-auth.conf /etc/apache2/sites-available/000-default.conf
    fi
    if [[ "$MOODLE_PERSISTED_FILE_SYNC" == 'true' ]]; then
        log "Starting background persisted file sync process..."
        (
            log "Logging in to Azure..."
            az login --identity --allow-no-subscriptions # --output none
            /app/file-sync-from-azure.sh restore
            /app/file-sync-generate-inventory.sh generate
            while true; do
                sleep 60
                /app/file-sync-to-azure.sh sync || log "Background sync failed"
                # Log in again to refresh token
                az login --identity --allow-no-subscriptions # --output none
            done
        ) &        
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
