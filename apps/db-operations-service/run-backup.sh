#!/bin/sh
set -e
set -x

echo "Authenticating with Azure using Managed Identity..."
az login --identity
echo "Authentication successful."

# --- Input Validation ---
# Checks for exactly 3 arguments.
if [ -z "$3" ]; then
  echo "Usage: $0 <database-name> <storage-account-name> <container-name>" >&2
  exit 1
fi

DATABASE_NAME=$1
STORAGE_ACCOUNT=$2
CONTAINER_NAME=$3
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
BACKUP_FILE="/tmp/${DATABASE_NAME}_${TIMESTAMP}.dump"

echo "Starting backup for database '$DATABASE_NAME'..."

# --- Resolve Password from Key Vault ---
# Uses the app's managed identity to get the secret.
# ACTUAL_DB_PASSWORD=$(az keyvault secret show --name "$DB_PASSWORD" --vault-name "$KEY_VAULT_NAME" --query value -o tsv)
# if [ -z "$ACTUAL_DB_PASSWORD" ]; then
#     echo "Error: Failed to retrieve password from Key Vault." >&2
#     exit 1
# fi

# --- Parse Connection String ---
# Extracts Host and Username from the key-value format.
DB_HOST=$(echo "$CONNECTIONSTRINGS__DEFAULTCONNECTION" | grep -o 'Host=[^;]*' | cut -d'=' -f2)
DB_USER=$(echo "$CONNECTIONSTRINGS__DEFAULTCONNECTION" | grep -o 'Username=[^;]*' | cut -d'=' -f2)

# --- 1. Execute Backup ---
export PGPASSWORD=$DB_PASSWORD
pg_dump -F c --verbose -h "$DB_HOST" -U "$DB_USER" -d "$DATABASE_NAME" > "$BACKUP_FILE"
unset PGPASSWORD
echo "Local backup created at $BACKUP_FILE."

# --- 2. Upload to Azure Blob Storage ---
az storage blob upload \
  --connection-string "$STORAGECONNECTIONSTRING" \
  --container-name "$CONTAINER_NAME" \
  --file "$BACKUP_FILE" \
  --name "$(basename "$BACKUP_FILE")"
echo "Upload to blob storage successful."

# --- 3. Clean Up ---
rm "$BACKUP_FILE"
echo "Process complete."