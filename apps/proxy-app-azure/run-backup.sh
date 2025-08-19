#!/bin/sh

# Exit immediately if a command exits with a non-zero status.
set -e

## --- Configuration ---
# This script expects the following environment variables to be set:
# CONNECTIONSTRINGS__DEFAULTCONNECTION: e.g., "Host=...;Database=$[DB_REPLACE_DB_NAME];Username=...;Password=$[DB_REPLACE_PASSWORD];..."
# DB_PASSWORD: The password for the database user.

## --- Input Validation ---
# Check if all three required arguments were provided.
if [ -z "$4" ]; then
  echo "Usage: $0 <database-name> <storage-account-name> <container-name> <backup-file>"
  exit 1
fi

DATABASE_NAME=$1
STORAGE_ACCOUNT=$2
CONTAINER_NAME=$3
BACKUP_FILE=$4

## --- Check for Environment Variables ---
if [ -z "$CONNECTIONSTRINGS__DEFAULTCONNECTION" ] || [ -z "$DB_PASSWORD" ]; then
  echo "Error: Please set the CONNECTIONSTRINGS__DEFAULTCONNECTION and DB_PASSWORD environment variables."
  exit 1
fi

## --- Parse the Connection String ---
echo "Parsing connection string template..."

# Extract the Host value
DB_HOST=$(echo "$CONNECTIONSTRINGS__DEFAULTCONNECTION" | grep -o 'Host=[^;]*' | cut -d'=' -f2)

# Extract the Username value
DB_USER=$(echo "$CONNECTIONSTRINGS__DEFAULTCONNECTION" | grep -o 'Username=[^;]*' | cut -d'=' -f2)

# Check if values were extracted successfully
if [ -z "$DB_HOST" ] || [ -z "$DB_USER" ]; then
    echo "Error: Could not parse Host and Username from CONNECTIONSTRINGS__DEFAULTCONNECTION."
    exit 1
fi

## --- 1. Execute the Local Backup ---
echo "Starting backup for database '$DATABASE_NAME' on host '$DB_HOST'..."
echo "Local temporary file: $BACKUP_FILE"

# Set PGPASSWORD so the password isn't exposed on the command line.
export PGPASSWORD=$DB_PASSWORD

# Use individual parameters for pg_dump
pg_dump -F c --verbose \
  -h "$DB_HOST" \
  -U "$DB_USER" \
  -d "$DATABASE_NAME" > "$BACKUP_FILE"

# Unset the password variable immediately after use
unset PGPASSWORD

echo "Local backup complete."

## --- 2. Upload to Azure Blob Storage ---
echo "Uploading backup to Azure Storage Account '$STORAGE_ACCOUNT' in container '$CONTAINER_NAME'..."

# This command assumes the server has a Managed Identity with the 'Storage Blob Data Contributor' role.
az storage blob upload \
  --account-name "$STORAGE_ACCOUNT" \
  --container-name "$CONTAINER_NAME" \
  --file "$BACKUP_FILE" \
  --name "$BACKUP_FILE" \
  --auth-mode login

echo "Upload successful."

## --- 3. Clean Up Local File ---
echo "Removing local backup file: $BACKUP_FILE"
rm "$BACKUP_FILE"

echo "Process complete!"