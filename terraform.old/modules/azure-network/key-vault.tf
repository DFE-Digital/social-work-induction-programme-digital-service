# Create Key Vault
data "azurerm_client_config" "az_config" {}

resource "azurerm_key_vault" "kv" {
  name                        = "${var.resource_name_prefix}-kv-01"
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
  }

  #checkov:skip=CKV_AZURE_109:Access Policies configured
  #checkov:skip=CKV_AZURE_189:Access Policies configured
  #checkov:skip=CKV2_AZURE_32:VNET configuration adequate
}

# Access Policy for GitHub Actions
resource "azurerm_key_vault_access_policy" "kv_gh_ap" {
  key_vault_id = azurerm_key_vault.kv.id
  tenant_id    = data.azurerm_client_config.az_config.tenant_id
  object_id    = data.azurerm_client_config.az_config.object_id

  key_permissions = [
    "Create",
    "Delete",
    "Get",
    "UnwrapKey",
    "WrapKey",
    "Recover",
    "Purge",
    "Update",
    "GetRotationPolicy",
    "SetRotationPolicy"
  ]

  secret_permissions = ["List", "Get", "Set", "Recover"]

  certificate_permissions = [
    "Create",
    "Get",
    "GetIssuers",
    "Import",
    "List",
    "ListIssuers",
    "ManageContacts",
    "ManageIssuers",
    "SetIssuers",
    "Update"
  ]

  lifecycle {
    ignore_changes = [object_id]
  }
}
