locals {
  principal_certificate_permissions = [
    "Create",
    "Delete",
    "DeleteIssuers",
    "Get",
    "GetIssuers",
    "Import",
    "List",
    "ListIssuers",
    "ManageContacts",
    "ManageIssuers",
    "SetIssuers",
    "Update",
    "Purge",
  ]

  principal_key_permissions = [
    "Backup",
    "Create",
    "Decrypt",
    "Delete",
    "Encrypt",
    "Get",
    "Import",
    "List",
    "Purge",
    "Recover",
    "Restore",
    "Sign",
    "UnwrapKey",
    "Update",
    "Verify",
    "WrapKey",
    "Release",
    "Rotate",
    "GetRotationPolicy",
    "SetRotationPolicy",
  ]

  principal_secret_permissions = [
    "Backup",
    "Delete",
    "Get",
    "List",
    "Purge",
    "Recover",
    "Restore",
    "Set",
  ]

  service_principals = {
    # For convenience, we give the delivery team read / write access to secrets in non-prod envs
    # Note that they can do this anyway manually, but Terraform will reset to known state
    delivery_team_user_group = {
      object_id          = "42d30dac-805f-42e7-82fc-6b70f5c4649b" # s205-earlycareerframeworkforsocialwork-Delivery Team
      assign_permissions = var.assign_delivery_team_key_vault_permissions
    }
    github_actions = {
      object_id          = data.azurerm_client_config.az_config.object_id
      assign_permissions = true
    }
  }
}

resource "azurerm_key_vault" "kv" {
  name                        = "${var.resource_name_prefix}-kv-primary"
  resource_group_name         = azurerm_resource_group.rg_primary.name
  location                    = var.location
  tenant_id                   = data.azurerm_client_config.az_config.tenant_id
  enabled_for_disk_encryption = true
  soft_delete_retention_days  = 7
  purge_protection_enabled    = var.kv_purge_protection_enabled
  sku_name                    = var.key_vault_sku

  lifecycle {
    ignore_changes = [tags]
  }

  #checkov:skip=CKV_AZURE_109:Access Policies configured
  #checkov:skip=CKV_AZURE_189:Access Policies configured
  #checkov:skip=CKV2_AZURE_32:VNET configuration adequate
}

resource "azurerm_key_vault_access_policy" "kv_gh_ap" {
  for_each     = { for k, v in local.service_principals : k => v if v.assign_permissions }
  key_vault_id = azurerm_key_vault.kv.id
  tenant_id    = data.azurerm_client_config.az_config.tenant_id
  object_id    = each.value.object_id

  key_permissions         = local.principal_key_permissions
  secret_permissions      = local.principal_secret_permissions
  certificate_permissions = local.principal_certificate_permissions

  lifecycle {
    ignore_changes = [object_id]
  }
}
