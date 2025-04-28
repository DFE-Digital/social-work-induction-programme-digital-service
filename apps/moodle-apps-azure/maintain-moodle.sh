#!/bin/bash

# PGPASSWORD is used by psql for authentication.
export PGPASSWORD="${POSTGRES_PASSWORD}"
PG_CONN="psql -h ${MOODLE_DB_HOST} -U ${POSTGRES_USER} -d ${POSTGRES_DB}"

echo "Checking if custom Moodle schema/table 'moodle_migration.moodle_migration' exists..."
MIGRATION_TABLE_EXISTS=$($PG_CONN -tAc "SELECT to_regclass('moodle_migration.moodle_migration');" | xargs)

echo "Retrieving last successful install entry..."

if [[ -z "$TABLE_EXISTS" || "$TABLE_EXISTS" == "NULL" ]]; then
    LAST_SUCCESS=''
else
    # Retrieve the latest successful install entry as a formatted string, or return an empty string.
    LAST_SUCCESS=$($PG_CONN -tAc "SELECT to_char(migrated_at, 'YYYY-MM-DD HH24:MI:SS') || ' version ' || version FROM moodle_migration.moodle_migration WHERE success = true ORDER BY migrated_at DESC LIMIT 1;" | xargs)
fi

# Moodle maintains its latest version in /version.php
VERSION=$(awk -F"'" '/\$release[[:space:]]*=/ {print $2}' version.php)

if [[ -z "$LAST_SUCCESS" ]]; then
    echo "No successful install found; creating Moodle database from version $VERSION..."
    su -s /bin/sh www-data -c 'php admin/cli/install_database.php \
        --lang=en \
        --adminemail=$MOODLE_ADMIN_EMAIL \
        --adminpass=$MOODLE_ADMIN_PASSWORD \
        --shortname="$MOODLE_SITE_SHORTNAME" \
        --fullname="$MOODLE_SITE_FULLNAME" \
        --agree-license'

    if [ $? -eq 0 ]; then
        # Required to make sure plug-ins / themes are OK
        su -s /bin/sh www-data -c 'php admin/cli/upgrade.php --non-interactive'
    fi
    if [ $? -eq 0 ]; then
        
        if [[ -z "$TABLE_EXISTS" || "$TABLE_EXISTS" == "NULL" ]]; then
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
else
    echo "Last successful install entry: $LAST_SUCCESS"
    echo "Now upgrading to version $VERSION..."

    su -s /bin/sh www-data -c 'bin/php admin/cli/upgrade.php --non-interactive'

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
fi

wait $APACHE_PID