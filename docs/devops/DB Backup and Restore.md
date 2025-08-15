# Database Backup and Restore

This document outlines the process for backing up and restoring the database using the reusable GitHub Action workflow defined in `.github/workflows/db-backup-restore.yml`.

## Overview

The `db-backup-restore.yml` workflow provides two jobs: `backup` and `restore`. It is a reusable workflow (`workflow_call`) designed to be called by other workflows to manage database state for a specific environment.

## Backup Process (`backup`)

The `backup` job creates a compressed dump of a specified database and uploads it to Azure Blob Storage.

### Steps

1.  **Login to Azure:** Authenticates with Azure using the provided service principal credentials.
2.  **Fetch DB Credentials:** Retrieves the database username and password from Azure Key Vault.
3.  **Install PostgreSQL Client:** Installs the `postgresql-client` package required for `pg_dump`.
4.  **Perform Database Backup:**
    *   A timestamp is generated (`YYYYMMDDHHMMSS`).
    *   `pg_dump` is used to create a custom-format archive of the database specified in the `db_name` input.
    *   The backup file is named `<db_name>_backup_<timestamp>.dump`.
5.  **Upload to Azure Blob Storage:** The generated backup file is uploaded to the specified container in an Azure Blob Storage account.

## Restore Process (`restore`)

The `restore` job downloads a specified backup file from Azure Blob Storage and restores it to the target database.

**Important:** Restoring the database is a destructive operation. It will **drop the existing database** before recreating it from the backup file. This action should be used with caution.

### Steps

1.  **Login to Azure:** Authenticates with Azure.
2.  **Fetch DB Credentials:** Retrieves database credentials from Azure Key Vault.
3.  **Install PostgreSQL Client:** Installs `postgresql-client` for `psql` and `pg_restore`.
4.  **Download Backup:** Downloads the specified backup file (via the `backup_file_name` input) from Azure Blob Storage to a local file named `restore.dump`.
5.  **Restore Database:**
    *   Connects to the PostgreSQL server and drops the target database (`db_name`) if it exists.
    *   Creates a new, empty database with the same name.
    *   Uses `pg_restore` to restore the contents of the database from the downloaded `restore.dump` file. The `--no-owner` and `--no-privileges` flags are used to ensure the restoring user becomes the owner of the objects.

## Usage

To use this reusable workflow, you must call it from another workflow and provide the necessary inputs and secrets.

### Inputs

| Name               | Description                                                    | Required |
| ------------------ | -------------------------------------------------------------- | -------- |
| `job_to_run`       | The job to run: `backup` or `restore`.                         | Yes      |
| `db_name`          | The name of the database to back up or restore.                | Yes      |
| `backup_file_name` | The name of the backup file in Blob Storage to restore from.   | For `restore` |

### Secrets

The following secrets must be provided to the workflow:

*   `AZURE_CLIENT_ID`: The client ID for the Azure service principal.
*   `AZURE_TENANT_ID`: The tenant ID for the Azure service principal.
*   `AZURE_SUBSCRIPTION_ID`: The Azure subscription ID.
*   `KEY_VAULT_NAME`: The name of the Azure Key Vault containing secrets.
*   `DB_USER_SECRET_NAME`: The name of the secret in Key Vault for the database username.
*   `DB_PASSWORD_SECRET_NAME`: The name of the secret in Key Vault for the database password.
*   `AZURE_DB_HOST`: The hostname of the Azure PostgreSQL server.
*   `STORAGE_ACCOUNT_NAME`: The name of the Azure Storage Account for backups.
*   `STORAGE_CONTAINER_NAME`: The name of the container within the storage account.

### Example Caller Workflow

This workflow could be used to manually trigger a backup or restore for a given environment.

```yaml
name: 'Manual DB Backup and Restore'

on:
  workflow_dispatch:
    inputs:
      job_to_run:
        description: "The job to run (backup or restore)"
        required: true
        type: choice
        options:
          - backup
          - restore
      db_name:
        description: "The name of the database to backup/restore (e.g. moodle_d01)"
        required: true
        type: string
      backup_file_name:
        description: "The name of the backup file in Blob Storage to restore (only for restore job)"
        required: false
        type: string

jobs:
  db-backup-restore:
    uses: ./.github/workflows/db-backup-restore.yml
    with:
      job_to_run: ${{ github.event.inputs.job_to_run }}
      db_name: ${{ github.event.inputs.db_name }}
      backup_file_name: ${{ github.event.inputs.backup_file_name }}
    secrets:
      AZURE_CLIENT_ID: ${{ secrets.AZURE_CLIENT_ID }}
      AZURE_TENANT_ID: ${{ secrets.AZURE_TENANT_ID }}
      AZURE_SUBSCRIPTION_ID: ${{ secrets.AZURE_SUBSCRIPTION_ID }}
      KEY_VAULT_NAME: ${{ secrets.KEY_VAULT_NAME }}
      DB_USER_SECRET_NAME: ${{ secrets.DB_USER_SECRET_NAME }}
      DB_PASSWORD_SECRET_NAME: ${{ secrets.DB_PASSWORD_SECRET_NAME }}
      AZURE_DB_HOST: ${{ secrets.AZURE_DB_HOST }}
      STORAGE_ACCOUNT_NAME: ${{ secrets.STORAGE_ACCOUNT_NAME }}
      STORAGE_CONTAINER_NAME: ${{ secrets.STORAGE_CONTAINER_NAME }}
```
