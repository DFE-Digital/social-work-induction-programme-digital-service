resource "azurerm_storage_account" "sa" {
  name                             = var.webapp_storage_account_name
  resource_group_name              = var.resource_group
  location                         = var.location
  account_tier                     = "Standard"
  min_tls_version                  = "TLS1_2"
  account_replication_type         = "LRS"
  allow_nested_items_to_be_public  = false
  cross_tenant_replication_enabled = false
  shared_access_key_enabled        = true

  queue_properties {
    logging {
      delete                = true
      read                  = true
      write                 = true
      version               = "1.0"
      retention_policy_days = 10
    }
    hour_metrics {
      enabled               = true
      include_apis          = true
      version               = "1.0"
      retention_policy_days = 10
    }
    minute_metrics {
      enabled               = true
      include_apis          = true
      version               = "1.0"
      retention_policy_days = 10
    }
  }

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
  #checkov:skip=CKV_AZURE_40:Connection string dont need expiry date
}

resource "azurerm_key_vault_secret" "storage_connection_string" {
  name         = "Storage--ConnectionString"
  value        = azurerm_storage_account.sa.primary_connection_string
  key_vault_id = var.kv_id
  content_type = "connection string"
}
