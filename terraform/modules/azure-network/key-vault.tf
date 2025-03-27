# Create Key Vault
data "azurerm_client_config" "az_config" {}

resource "azurerm_key_vault" "kv" {
  name                        = "${var.resource_name_prefix}-kv"
  resource_group_name         = var.resource_group
  location                    = var.location
  tenant_id                   = data.azurerm_client_config.az_config.tenant_id
  enabled_for_disk_encryption = true
  soft_delete_retention_days  = 7
  purge_protection_enabled    = true
  sku_name                    = "standard"

  lifecycle {
    ignore_changes = [
      tags["Environment"],
      tags["Product"],
      tags["Service Offering"]
    ]
    create_before_destroy = true
  }

  #checkov:skip=CKV_AZURE_109:Access Policies configured
  #checkov:skip=CKV_AZURE_189:Access Policies configured
  #checkov:skip=CKV2_AZURE_32:VNET configuration adequate
}

resource "azurerm_key_vault" "kv" {
  name                        = "${var.resource_name_prefix}-kv"
  resource_group_name         = var.resource_group
  location                    = var.location
  tenant_id                   = data.azurerm_client_config.az_config.tenant_id
  enabled_for_disk_encryption = true
  soft_delete_retention_days  = 7
  purge_protection_enabled    = true
  sku_name                    = "standard"
  access_policy               = []
  enable_rbac_authorization   = true

  lifecycle {
    ignore_changes = [
      tags["Environment"],
      tags["Product"],
      tags["Service Offering"]
    ]
    create_before_destroy = true
  }

  #checkov:skip=CKV2_AZURE_32:VNET configuration adequate
}
