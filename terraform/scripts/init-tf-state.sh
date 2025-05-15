#!/bin/bash
set -euo pipefail

##########################
# Retry Helper           #
##########################

# Number of attempts and delay between them (in seconds)
RETRY_ATTEMPTS=${RETRY_ATTEMPTS:-3}
RETRY_DELAY=${RETRY_DELAY:-5}

retry_az() {
  local -i n=1
  local -i max=$RETRY_ATTEMPTS
  local delay=$RETRY_DELAY
  # Capture the command as an array
  local cmd=( "$@" )

  until "${cmd[@]}"; do
    if (( n >= max )); then
      echo "ERROR: Command failed after $n attempts: ${cmd[*]}" >&2
      return 1
    fi
    echo "WARN: Command failed (attempt $n/$max). Retrying in $delay s..." >&2
    sleep "$delay"
    ((n++))
  done
}

##########################
# Configuration Settings #
##########################

# Change these values as appropriate
LOCATION=$1
RESOURCE_GROUP=$2
STORAGE_ACCOUNT_NAME=$3  # must be unique and lower-case
CONTAINER_NAME=$4
STATE_BLOB_NAME=$5
# Remove the first 5 arguments
shift 5
# Capture remaining arguments as an array of tags
RESOURCE_TAGS=( "$@" )

echo "Location: $LOCATION"
echo "Resource Group: $RESOURCE_GROUP"
echo "Storage Account: $STORAGE_ACCOUNT_NAME"
echo "Container: $CONTAINER_NAME"
echo "State Blob: $STATE_BLOB_NAME"
echo "Resource Tags: ${RESOURCE_TAGS[*]}"

##########################
# Resource Group Check   #
##########################

echo "Terraform state: checking if resource group '$RESOURCE_GROUP' exists..."
if ! retry_az az group show --name "$RESOURCE_GROUP" &>/dev/null; then
    echo "Terraform state: resource group not found. Creating resource group $RESOURCE_GROUP..."
    retry_az az group create \
        --name "$RESOURCE_GROUP" \
        --location "$LOCATION" \
        -o none \
        --tags "${RESOURCE_TAGS[@]}"
else
    echo "Terraform state: resource group exists"
fi

##########################
# Storage Account Check  #
##########################

echo "Terraform state: checking if storage account '$STORAGE_ACCOUNT_NAME' exists in resource group '$RESOURCE_GROUP'..."
if ! retry_az az storage account show --resource-group "$RESOURCE_GROUP" --name "$STORAGE_ACCOUNT_NAME" &>/dev/null; then
    echo "Terraform state: storage account not found. Creating storage account $STORAGE_ACCOUNT_NAME..."
    retry_az az storage account create \
        --resource-group "$RESOURCE_GROUP" \
        --name "$STORAGE_ACCOUNT_NAME" \
        --location "$LOCATION" \
        --sku Standard_LRS \
        --https-only true \
        --min-tls-version TLS1_2 \
        --allow-blob-public-access false \
        -o none \
        --tags "${RESOURCE_TAGS[@]}"
else
    echo "Terraform state: storage account exists."
fi

echo "Terraform state: retrieving storage account key for storage account (first will be used): $STORAGE_ACCOUNT_NAME..."
# Retrieve the storage account key (we use the first key)
ACCOUNT_KEY=$(retry_az az storage account keys list \
    --resource-group "$RESOURCE_GROUP" \
    --account-name "$STORAGE_ACCOUNT_NAME" \
    --query "[0].value" -o tsv)

##########################
# Enable Blob Versioning #
##########################

# Check if versioning is enabled. We expect a "true" response if it is.
echo "Terraform state: checking if blob versioning is enabled for storage account '$STORAGE_ACCOUNT_NAME'..."
VERSIONING=$(retry_az az storage account blob-service-properties show \
    --account-name "$STORAGE_ACCOUNT_NAME" \
    --resource-group "$RESOURCE_GROUP" \
    --query "isVersioningEnabled" -o tsv 2>/dev/null || echo "false")

if [[ "$VERSIONING" != "true" ]]; then
    echo "Terraform state: blob versioning is not enabled. Enabling blob versioning on the storage account..."
    retry_az az storage account blob-service-properties update \
        --account-name "$STORAGE_ACCOUNT_NAME" \
        --resource-group "$RESOURCE_GROUP" \
        -o none \
        --enable-versioning true
else
    echo "Terraform state: blob versioning is already enabled."
fi

##########################
# Container Check        #
##########################

echo "Terraform state: checking if container '$CONTAINER_NAME' exists in storage account '$STORAGE_ACCOUNT_NAME'..."
CONTAINER_EXISTS=$(retry_az az storage container exists \
    --name "$CONTAINER_NAME" \
    --account-name "$STORAGE_ACCOUNT_NAME" \
    --account-key "$ACCOUNT_KEY" \
    --query "exists" -o tsv)

if [[ "$CONTAINER_EXISTS" != "true" ]]; then
    echo "Terraform state: container not found. Creating container..."
    retry_az az storage container create \
        --name "$CONTAINER_NAME" \
        --account-name "$STORAGE_ACCOUNT_NAME" \
        --account-key "$ACCOUNT_KEY" \
        -o none \
        --public-access off
else
    echo "Terraform state: container exists."
fi

echo "Terraform state: all resources are in place."
