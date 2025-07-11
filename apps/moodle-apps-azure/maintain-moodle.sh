#!/bin/bash
set -euo pipefail

WEB_SERVICE_SETUP_FILE="/var/www/html/public/setup_moodle_webservice.php"

cleanup() {
    if [ -f "$WEB_SERVICE_SETUP_FILE" ]; then
        rm "$WEB_SERVICE_SETUP_FILE"
    fi
}

trap cleanup EXIT

# PGPASSWORD is used by psql for authentication.
export PGPASSWORD="${POSTGRES_PASSWORD}"
PG_CONN="psql -h ${MOODLE_DB_HOST} -U ${POSTGRES_USER} -d ${POSTGRES_DB} -p 5432"

echo "Checking if custom Moodle schema/table 'moodle_migration.moodle_migration' exists..."
MIGRATION_TABLE_EXISTS=$($PG_CONN -tAc "SELECT to_regclass('moodle_migration.moodle_migration');" | xargs)

echo "Retrieving last successful install entry..."

if [[ -z "$MIGRATION_TABLE_EXISTS" || "$MIGRATION_TABLE_EXISTS" == "NULL" ]]; then
    LAST_SUCCESS=''
else
    # Retrieve the latest successful install entry as a formatted string, or return an empty string.
    LAST_SUCCESS=$($PG_CONN -tAc "SELECT to_char(migrated_at, 'YYYY-MM-DD HH24:MI:SS') || ' version ' || version FROM moodle_migration.moodle_migration WHERE success = true ORDER BY migrated_at DESC LIMIT 1;" | xargs)
fi

cd /var/www/html/public

# Moodle maintains its latest version in /version.php
VERSION=$(awk -F"'" '/\$release[[:space:]]*=/ {print $2}' version.php)

if [[ -z "$LAST_SUCCESS" ]]; then
    echo "No successful install found; creating Moodle database from version $VERSION..."
    su -s /bin/sh www-data -c 'php admin/cli/install_database.php \
        --lang=en \
        --adminuser=$MOODLE_ADMIN_USER \
        --adminemail=$MOODLE_ADMIN_EMAIL \
        --adminpass=$MOODLE_ADMIN_PASSWORD \
        --shortname="$MOODLE_SITE_SHORTNAME" \
        --fullname="$MOODLE_SITE_FULLNAME" \
        --agree-license'

    if [ $? -eq 0 ]; then
        
        if [[ -z "$MIGRATION_TABLE_EXISTS" || "$MIGRATION_TABLE_EXISTS" == "NULL" ]]; then
            echo "Schema/table does not exist; creating schema 'moodle_migration' and table 'moodle_migration'."
            $PG_CONN <<EOF
        CREATE SCHEMA IF NOT EXISTS moodle_migration;
        CREATE TABLE IF NOT EXISTS moodle_migration.moodle_migration (
            migrated_at TIMESTAMPTZ NOT NULL DEFAULT now(),
            version VARCHAR(50) NOT NULL,
            install_status TEXT,
            success BOOLEAN
        );
EOF
        else
            echo "Custom Moodle migration schema / table exists"
        fi

        echo "Install script completed successfully. Recording success in the migration table..."
        $PG_CONN <<EOF
INSERT INTO moodle_migration.moodle_migration (version, install_status, success)
VALUES ('$VERSION', 'Installation successful', true);
EOF
    else
        echo "Install script failed"
    fi
fi

if [ $? -eq 0 ]; then
    echo "Last successful install entry: $LAST_SUCCESS"
    echo "Now running web service setup..."

    cp /app/setup_moodle_webservice.php $WEB_SERVICE_SETUP_FILE
    su -s /bin/sh www-data -c 'php setup_moodle_webservice.php \
        --username="$MOODLE_WEB_SERVICE_USER" \
        --password="$MOODLE_WEB_SERVICE_USER_PASSWORD" \
        --email="$MOODLE_WEB_SERVICE_USER_EMAIL" \
        --servicename="$MOODLE_WEB_SERVICE_NAME" \
        --token="$MOODLE_WEB_SERVICE_TOKEN"'
fi

if [ $? -eq 0 ]; then
    echo "Now running install process for OIDC plugin..."
    # Make sure Moosh doesn't check for www-data ownership of moodledata directory. Haven't been able to configure 
    # Azure Files to do this yet.
    export MOOSH_COMMAND_LINE_ARGS=-n
    su -s /bin/sh www-data -c '/app/install-oidc-plugin config \
        skip-download \
        $AUTH_SERVICE_CLIENT_ID \
        $AUTH_SERVICE_CLIENT_SECRET \
        $AUTH_SERVICE_END_POINT \
        $AUTH_SERVICE_TOKEN_END_POINT \
        $AUTH_SERVICE_LOGOUT_URI'
fi

if [ $? -eq 0 ]; then
    echo "Now running Moodle upgrade process..."
    su -s /bin/sh www-data -c 'php admin/cli/upgrade.php --non-interactive'
fi

if [ $? -eq 0 ]; then
    echo "Upgrade script completed successfully. Recording success in the migration table..."
    $PG_CONN <<EOF
INSERT INTO moodle_migration.moodle_migration (version, install_status, success)
VALUES ('$VERSION', 'Upgrade successful', true);
EOF
else
    echo "Upgrade script failed. Recording failure in the migration table and exiting..."
    $PG_CONN <<EOF
INSERT INTO moodle_migration.moodle_migration (version, install_status, success)
VALUES ('$VERSION', 'Upgrade failed', false);
EOF
fi
