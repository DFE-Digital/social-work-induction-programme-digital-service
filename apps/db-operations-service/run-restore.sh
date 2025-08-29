#!/bin/sh

# Exit immediately if a command exits with a non-zero status.
set -e
set -x

echo "Authenticating with Azure using Managed Identity..."
az login --identity
echo "Authentication successful."

## --- Configuration ---
# This script expects the following environment variables to be set:
# CONNECTIONSTRINGS__DEFAULTCONNECTION: e.g., "Host=...;Database=$[DB_REPLACE_DB_NAME];Username=...;Password=$[DB_REPLACE_PASSWORD];..."
# DB_PASSWORD: The password for the database user.

## --- Input Validation ---
# Check if all four required arguments were provided.
if [ -z "$4" ]; then
  echo "Usage: $0 <database-name> <storage-account-name> <container-name> <backup-file-name>"
  exit 1
fi

DATABASE_NAME=$1
STORAGE_ACCOUNT=$2
CONTAINER_NAME=$3
BACKUP_FILE=$4 # The specific .dump file to restore from

## --- Check for Environment Variables ---
if [ -z "$CONNECTIONSTRINGS__DEFAULTCONNECTION" ] || [ -z "$DB_PASSWORD" ]; then
  echo "Error: Please set the CONNECTIONSTRINGS__DEFAULTCONNECTION and DB_PASSWORD environment variables."
  exit 1
fi

## --- Parse the Connection String ---
echo "Parsing connection string template..."

# Extract the Host and Username values
DB_HOST=$(echo "$CONNECTIONSTRINGS__DEFAULTCONNECTION" | grep -o 'Host=[^;]*' | cut -d'=' -f2)
DB_USER=$(echo "$CONNECTIONSTRINGS__DEFAULTCONNECTION" | grep -o 'Username=[^;]*' | cut -d'=' -f2)

if [ -z "$DB_HOST" ] || [ -z "$DB_USER" ]; then
    echo "Error: Could not parse Host and Username from CONNECTIONSTRINGS__DEFAULTCONNECTION."
    exit 1
fi

## --- 1. Download Backup from Azure Blob Storage ---
echo "Downloading backup file '$BACKUP_FILE' from container '$CONTAINER_NAME'..."

# This command assumes the server has a Managed Identity with the 'Storage Blob Data Contributor' role.
az storage blob download \
  --account-name "$STORAGE_ACCOUNT" \
  --container-name "$CONTAINER_NAME" \
  --name "$BACKUP_FILE" \
  --file "$BACKUP_FILE" \
  --auth-mode login

echo "Download successful."

## --- 2. Prepare Target Database ---
echo "Preparing database '$DATABASE_NAME' for restore..."

# Set PGPASSWORD so the password isn't exposed on the command line.
export PGPASSWORD=$DB_PASSWORD

# Connect to the default 'postgres' database to drop and recreate the target database
psql -h "$DB_HOST" -U "$DB_USER" -d postgres -c "DROP DATABASE IF EXISTS \"$DATABASE_NAME\";"
psql -h "$DB_HOST" -U "$DB_USER" -d postgres -c "CREATE DATABASE \"$DATABASE_NAME\";"

echo "Database '$DATABASE_NAME' has been recreated."

## --- 3. Execute the Restore ---
echo "Restoring data from '$BACKUP_FILE'..."

# Use pg_restore to load the data into the newly created database
pg_restore --verbose \
  -h "$DB_HOST" \
  -U "$DB_USER" \
  -d "$DATABASE_NAME" \
  "$BACKUP_FILE"

# Unset the password variable immediately after use
unset PGPASSWORD

echo "Restore complete."

## --- 4. Clean Up Local File ---
echo "Removing local backup file: $BACKUP_FILE"
rm "$BACKUP_FILE"

echo "Process complete!"