resource "azurerm_storage_account" "sa_app_blob_storage" {
  name                             = "${var.resource_name_prefix}${var.webapp_storage_account_name}"
  resource_group_name              = azurerm_resource_group.rg_primary.name
  location                         = var.location
  account_tier                     = var.blob_storage_account_tier
  min_tls_version                  = "TLS1_2"
  account_replication_type         = var.storage_redundancy
  allow_nested_items_to_be_public  = false
  cross_tenant_replication_enabled = false
  shared_access_key_enabled        = true

  blob_properties {
    delete_retention_policy {
      days = 7
    }
    container_delete_retention_policy {
      days = 7
    }
  }

  tags = var.tags

  lifecycle {
    ignore_changes = [
      tags["Environment"],
      tags["Product"],
      tags["Service Offering"]
    ]
  }

  #checkov:skip=CKV_AZURE_206:GRS not required
  #checkov:skip=CKV_AZURE_59:Argument has been deprecated
  #checkov:skip=CKV2_AZURE_18:Microsoft Managed keys are sufficient
  #checkov:skip=CKV2_AZURE_1:Microsoft Managed keys are sufficient
  #checkov:skip=CKV2_AZURE_38:Soft-delete not required
  #checkov:skip=CKV2_AZURE_33:VNet not configured
  #checkov:skip=CKV2_AZURE_41:SAS keys will be rotated
  #checkov:skip=CKV2_AZURE_40:Shared access key are sufficient
  #checkov:skip=CKV_AZURE_33:Argument has been deprecated
}

resource "azurerm_key_vault_secret" "blob_storage_connection_string" {
  name         = "BlobStorage-ConnectionString"
  value        = azurerm_storage_account.sa_app_blob_storage.primary_connection_string
  key_vault_id = azurerm_key_vault.kv.id
  content_type = "storage connection string"
  depends_on   = [azurerm_key_vault_access_policy.kv_gh_ap]

  #checkov:skip=CKV_AZURE_41:Connection string dont need expiry date
}

resource "azurerm_storage_account" "sa_app_file_storage" {
  name                             = "${var.resource_name_prefix}safiles"
  resource_group_name              = azurerm_resource_group.rg_primary.name
  location                         = var.location
  account_tier                     = var.file_storage_account_tier
  account_kind                     = var.file_storage_account_tier == "Premium" ? "FileStorage" : "StorageV2"
  min_tls_version                  = "TLS1_2"
  account_replication_type         = var.storage_redundancy
  allow_nested_items_to_be_public  = false
  cross_tenant_replication_enabled = false
  large_file_share_enabled         = true
  shared_access_key_enabled        = true

  tags = var.tags

  lifecycle {
    ignore_changes = [
      tags["Environment"],
      tags["Product"],
      tags["Service Offering"]
    ]
  }

  #checkov:skip=CKV_AZURE_206:GRS not required
  #checkov:skip=CKV_AZURE_59:Argument has been deprecated
  #checkov:skip=CKV2_AZURE_18:Microsoft Managed keys are sufficient
  #checkov:skip=CKV2_AZURE_1:Microsoft Managed keys are sufficient
  #checkov:skip=CKV2_AZURE_38:Soft-delete not required
  #checkov:skip=CKV2_AZURE_33:VNet not configured
  #checkov:skip=CKV2_AZURE_41:SAS keys will be rotated
  #checkov:skip=CKV2_AZURE_40:Shared access key are sufficient
  #checkov:skip=CKV_AZURE_33:Argument has been deprecated
}

resource "azurerm_key_vault_secret" "file_storage_connection_string" {
  name         = "FileStorage-ConnectionString"
  value        = azurerm_storage_account.sa_app_file_storage.primary_connection_string
  key_vault_id = azurerm_key_vault.kv.id
  content_type = "storage connection string"
  depends_on   = [azurerm_key_vault_access_policy.kv_gh_ap]

  #checkov:skip=CKV_AZURE_41:Connection string dont need expiry date
}

resource "azurerm_key_vault_secret" "file_storage_access_key" {
  name         = "FileStorage-AccessKey"
  value        = azurerm_storage_account.sa_app_file_storage.primary_access_key
  key_vault_id = azurerm_key_vault.kv.id
  content_type = "storage access key"
  depends_on   = [azurerm_key_vault_access_policy.kv_gh_ap]

  #checkov:skip=CKV_AZURE_41:Connection string dont need expiry date
}

resource "azurerm_storage_account" "sa_db_backup_blob_storage" {
  name                             = "${var.resource_name_prefix}${var.db_backup_blob_sa_name}"
  resource_group_name              = azurerm_resource_group.rg_primary.name
  location                         = var.location
  account_tier                     = var.blob_storage_account_tier
  account_replication_type         = var.storage_redundancy
  min_tls_version                  = "TLS1_2"
  allow_nested_items_to_be_public  = false
  cross_tenant_replication_enabled = false
  shared_access_key_enabled        = true

  blob_properties {
    delete_retention_policy {
      days = 7
    }
    container_delete_retention_policy {
      days = 7
    }
  }

  tags = var.tags

  lifecycle {
    ignore_changes = [
      tags["Environment"],
      tags["Product"],
      tags["Service Offering"]
    ]
  }

  #checkov:skip=CKV_AZURE_206:GRS not required
  #checkov:skip=CKV_AZURE_59:Argument has been deprecated
  #checkov:skip=CKV2_AZURE_18:Microsoft Managed keys are sufficient
  #checkov:skip=CKV2_AZURE_1:Microsoft Managed keys are sufficient
  #checkov:skip=CKV2_AZURE_38:Soft-delete not required
  #checkov:skip=CKV2_AZURE_33:VNet not configured
  #checkov:skip=CKV2_AZURE_41:SAS keys will be rotated
  #checkov:skip=CKV2_AZURE_40:Shared access key are sufficient
  #checkov:skip=CKV_AZURE_33:Argument has been deprecated
}

resource "azurerm_storage_container" "moodle_container" {
  name                  = "${var.resource_name_prefix}-moodle-backups"
  storage_account_id    = azurerm_storage_account.sa_db_backup_blob_storage.id
  container_access_type = "private" # Keep backups private

  #checkov:skip=CKV2_AZURE_21:Don't need storage loggin to be enabled
}

resource "azurerm_storage_container" "um_container" {
  name                  = "${var.resource_name_prefix}-usermanagement-backups"
  storage_account_id    = azurerm_storage_account.sa_db_backup_blob_storage.id
  container_access_type = "private" # Keep backups private

  #checkov:skip=CKV2_AZURE_21:Don't need storage loggin to be enabled
}

resource "azurerm_key_vault_secret" "db_backup_blob_storage_connection_string" {
  name         = "DbBackupBlobStorage-ConnectionString"
  value        = azurerm_storage_account.sa_db_backup_blob_storage.primary_connection_string
  key_vault_id = azurerm_key_vault.kv.id
  content_type = "storage connection string"
  depends_on   = [azurerm_key_vault_access_policy.kv_gh_ap]

  #checkov:skip=CKV_AZURE_41:Connection string dont need expiry date
}

resource "azurerm_storage_account" "sa_moodle_data" {
  name                            = "${var.resource_name_prefix}samoodledata"
  resource_group_name             = azurerm_resource_group.rg_primary.name
  location                        = var.location
  account_tier                    = "Premium"
  account_kind                    = "FileStorage"
  account_replication_type        = "LRS"
  allow_nested_items_to_be_public = false
  nfsv3_enabled                   = false

  # When mounting an NFS share, you'll need to ensure that Secure Transfer Required 
  # is disabled on the storage account. App Service doesn't support mounting NFS shares 
  # when this is enabled. It uses port 2409 and virtual network integration and private 
  # endpoints as the security measure.
  https_traffic_only_enabled = false

  tags = var.tags

  lifecycle {
    ignore_changes = [
      tags["Environment"],
      tags["Product"],
      tags["Service Offering"]
    ]
  }

  #checkov:skip=CKV_AZURE_206:GRS not required
  #checkov:skip=CKV_AZURE_59:Argument has been deprecated
  #checkov:skip=CKV2_AZURE_18:Microsoft Managed keys are sufficient
  #checkov:skip=CKV2_AZURE_1:Microsoft Managed keys are sufficient
  #checkov:skip=CKV2_AZURE_38:Soft-delete not required
  #checkov:skip=CKV2_AZURE_33:VNet not configured
  #checkov:skip=CKV2_AZURE_41:SAS keys will be rotated
  #checkov:skip=CKV2_AZURE_40:Shared access key are sufficient
  #checkov:skip=CKV_AZURE_33:Argument has been deprecated
}

resource "azurerm_storage_share" "moodle_data_share" {
  name               = "${var.resource_name_prefix}-ss-moodle-data"
  storage_account_id = azurerm_storage_account.sa_moodle_data.id
  quota              = 100
  enabled_protocol   = "NFS"

  acl {
    id = "default"
    access_policy {
      permissions = "rwdl" # Read Write Delete List
    }
  }
}
