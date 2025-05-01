#!/bin/bash

# Support SSH for troubleshooting
echo "Starting SSH..."
/usr/sbin/sshd

# Save non-sensitive environment variables to file (will be used by SSH install / update session)
/app/save-env.sh /app/env.txt POSTGRES_PASSWORD MOOLE_ADMIN_PASSWORD

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
fi

# exec so Apache gets PID 1 and handles signals cleanly
echo "Starting Apache..."
exec apache2-foreground
