#!/bin/bash

# Support SSH for troubleshooting
echo "Starting SSH..."
/usr/sbin/sshd

# Save non-sensitive environment variables to file (will be used by SSH install / update session)
/app/save-env.sh /app/env.txt POSTGRES_PASSWORD MOODLE_ADMIN_PASSWORD

# The Moodle container will operate in one of 2 modes. Either full-blown Moodle instance, or 
# a cut-down version which just runs the cron jobs

echo "Cron configuration, IS_CRON_JOB_ONLY: $IS_CRON_JOB_ONLY"
if [[ "$IS_CRON_JOB_ONLY" == 'true' ]]; then
    echo "Switching to cron-only mode..."
    cp /app/apache-config-cron.conf /etc/apache2/sites-available/000-default.conf
    cp /app/moodle-cron /etc/cron.d/moodle-cron
    chmod 0644 /etc/cron.d/moodle-cron
    mv /var/www/html/public/version.txt /var/www/html/cron/version.txt
    echo "Starting cron daemon..."
    /usr/sbin/cron
else 
    echo "This will be a full Moodle instance..."
    if [[ "$BASIC_AUTH_ENABLED" == 'true' ]]; then
        # Configure basic auth to restrict access / prevent Moodle from being indexed
        echo "Configuring basic auth..."
        htpasswd -b -c /etc/apache2/.htpasswd "$BASIC_AUTH_USER" "$BASIC_AUTH_PASSWORD" > /dev/null 2>&1
        cp /app/apache-config-moodle-basic-auth.conf /etc/apache2/sites-available/000-default.conf
    fi
fi

cd /var/www/html/public
if [[ "$MOODLE_SWITCH_OFF_OAUTH" == 'true' ]]; then
    echo "Switching off OAuth..."
    echo "Disabling oidc authentication method in Moodle config..."
    su -s /bin/sh www-data -c 'moosh auth-manage disable oidc'
    echo "Enabling email-based self-registration..."
    su -s /bin/sh www-data -c 'moosh auth-manage enable email'
else
    echo "Switching on OAuth..."
    echo "Enabling oidc authentication method in Moodle config..."
    su -s /bin/sh www-data -c 'moosh auth-manage enable oidc'
    echo "Disabling email-based self-registration..."
    su -s /bin/sh www-data -c 'moosh auth-manage disable email'
fi
su -s /bin/sh www-data -c 'php admin/cli/purge_caches.php'

# exec so Apache gets PID 1 and handles signals cleanly - will be serving as user www-data
echo "Starting Apache..."
exec apache2-foreground
