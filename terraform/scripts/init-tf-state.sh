#!/bin/bash
set -euo pipefail

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
RESOURCE_TAGS=("$@")

echo "Location: $LOCATION"
echo "Resource Group: $RESOURCE_GROUP"
echo "Storage Account: $STORAGE_ACCOUNT_NAME"
echo "Container: $CONTAINER_NAME"
echo "State Blob: $STATE_BLOB_NAME"
echo "Resource Tags: ${RESOURCE_TAGS[@]}"

##########################
# Resource Group Check   #
##########################

echo "Terraform state: checking if resource group '$RESOURCE_GROUP' exists..."
if ! az group show --name "$RESOURCE_GROUP" &>/dev/null; then
    echo "Terraform state: resource group not found. Creating resource group $RESOURCE_GROUP..."
    az group create \
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
if ! az storage account show --resource-group "$RESOURCE_GROUP" --name "$STORAGE_ACCOUNT_NAME" &>/dev/null; then
    echo "Terraform state: storage account not found. Creating storage account $STORAGE_ACCOUNT_NAME..."
    az storage account create \
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
ACCOUNT_KEY=$(az storage account keys list \
    --resource-group "$RESOURCE_GROUP" \
    --account-name "$STORAGE_ACCOUNT_NAME" \
    --query "[0].value" -o tsv)

##########################
# Enable Blob Versioning #
##########################

# Check if versioning is enabled. We expect a "true" response if it is.
echo "Terraform state: checking if blob versioning is enabled for storage account '$STORAGE_ACCOUNT_NAME'..."
VERSIONING=$(az storage account blob-service-properties show \
    --account-name "$STORAGE_ACCOUNT_NAME" \
    --resource-group "$RESOURCE_GROUP" \
    --query "isVersioningEnabled" -o tsv 2>/dev/null || echo "false")

if [[ "$VERSIONING" != "true" ]]; then
    echo "Terraform state: blob versioning is not enabled. Enabling blob versioning on the storage account..."
    az storage account blob-service-properties update \
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
CONTAINER_EXISTS=$(az storage container exists \
    --name "$CONTAINER_NAME" \
    --account-name "$STORAGE_ACCOUNT_NAME" \
    --account-key "$ACCOUNT_KEY" \
    --query "exists" -o tsv)

if [ "$CONTAINER_EXISTS" != "true" ]; then
    echo "Terraform state: container not found. Creating container..."
    az storage container create \
        --name "$CONTAINER_NAME" \
        --account-name "$STORAGE_ACCOUNT_NAME" \
        --account-key "$ACCOUNT_KEY" \
        -o none \
        --public-access off
else
    echo "Terraform state: container exists."
fi

##########################
# Terraform State Blob   #
##########################

# echo "Checking if Terraform state blob '$STATE_BLOB_NAME' exists in container '$CONTAINER_NAME'..."
# BLOB_EXISTS=$(az storage blob exists \
#     --container-name "$CONTAINER_NAME" \
#     --name "$STATE_BLOB_NAME" \
#     --account-name "$STORAGE_ACCOUNT_NAME" \
#     --account-key "$ACCOUNT_KEY" \
#     --query "exists" -o tsv)

# if [ "$BLOB_EXISTS" != "true" ]; then
#     echo "Terraform state: blob not found. Creating an initial state file..."
#     # Create an empty state file (using {} as an empty state in JSON)
#     echo "{}" > empty_tf_state.json

#     az storage blob upload \
#         --container-name "$CONTAINER_NAME" \
#         --file empty_tf_state.json \
#         --name "$STATE_BLOB_NAME" \
#         --account-name "$STORAGE_ACCOUNT_NAME" \
#         --account-key "$ACCOUNT_KEY" \
#         --overwrite

#     rm empty_tf_state.json
# else
#     echo "Terraform state: blob exists."
# fi

echo "Terraform state: all resources are in place."
