#!/bin/bash
set -euo pipefail

# Get env vars in the Dockerfile to show up in the SSH session
eval $(printenv | sed -n "s/^\([^=]\+\)=\(.*\)$/export \1=\2/p" | sed 's/"/\\\"/g' | sed '/=/s//="/' | sed 's/$/"/' >> /etc/profile)

# Suport SSH for troubleshooting
echo "Starting SSH ..."
/usr/sbin/sshd

# Construct the PostgreSQL connection using environment variables.
# Required env variables: POSTGRES_DB, POSTGRES_USER, POSTGRES_PASSWORD, MOODLE_DB_HOST.
# PGPASSWORD is used by psql for authentication.
export PGPASSWORD="${POSTGRES_PASSWORD}"
PG_CONN="psql -h ${MOODLE_DB_HOST} -U ${POSTGRES_USER} -d ${POSTGRES_DB}"

echo "Checking if custom Moodle schema/table 'moodle_migration.moodle_migration' exists..."
TABLE_EXISTS=$($PG_CONN -tAc "SELECT to_regclass('moodle_migration.moodle_migration');" | xargs)

if [[ -z "$TABLE_EXISTS" || "$TABLE_EXISTS" == "NULL" ]]; then
    echo "Schema/table does not exist; creating schema 'moodle_migration' and table 'moodle_migration'."
    $PG_CONN <<'EOF'
CREATE SCHEMA IF NOT EXISTS moodle_migration;
CREATE TABLE IF NOT EXISTS moodle_migration.moodle_migration (
    migrated_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    version VARCHAR(50) NOT NULL,
    install_status TEXT,
    success BOOLEAN
);
EOF
    TABLE_EXISTED=false
else
    echo "Custom Moodle migration schema / table exists"
    TABLE_EXISTED=true
fi

echo "Retrieving last successful install entry..."

if [ "$TABLE_EXISTED" = "true" ]; then
    # Retrieve the latest successful install entry as a formatted string, or return an empty string.
    LAST_SUCCESS=$($PG_CONN -tAc "SELECT to_char(migrated_at, 'YYYY-MM-DD HH24:MI:SS') || ' version ' || version FROM moodle_migration.moodle_migration WHERE success = true ORDER BY migrated_at DESC LIMIT 1;" | xargs)
else
    LAST_SUCCESS=''
fi

# Moodle maintains its latest version in /version.php
VERSION=$(awk -F"'" '/\$release[[:space:]]*=/ {print $2}' version.php)

if [[ -z "$LAST_SUCCESS" ]]; then
    echo "No successful install found; creating Moodle database from version $VERSION..."

    sudo -u www-data /usr/bin/php admin/cli/install_database.php --lang=cs --adminpass=$MOODLE_ADMIN_PASSWORD --agree-license

    if [ $? -eq 0 ]; then

        echo "Install script completed successfully. Recording success in the migration table..."
        $PG_CONN <<EOF
INSERT INTO moodle_migration.moodle_migration (version, install_status, success)
VALUES ('$VERSION', 'Installation successful', true);
EOF
    else
        echo "Install script failed. Recording failure in the migration table and exiting..."
        $PG_CONN <<EOF
INSERT INTO moodle_migration.moodle_migration (version, install_status, success)
VALUES ('$VERSION', 'Installation failed', false);
EOF
    fi
else
    echo "Last successful install entry: $LAST_SUCCESS"
    echo "Now upgrading to version $VERSION..."

    sudo -u www-data /usr/bin/php admin/cli/upgrade.php

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

# Prevent container exit (or replace with your main process startup command).
pause
