module "stack" {
  source = "./modules/environment-stack"

  environment                                = var.environment
  location                                   = var.azure_region
  primary_resource_group                     = var.primary_resource_group
  resource_name_prefix                       = var.resource_name_prefix
  kv_purge_protection_enabled                = var.kv_purge_protection_enabled
  asp_sku_moodle                             = var.asp_sku_moodle
  admin_enabled                              = var.admin_enabled
  webapp_storage_account_name                = var.webapp_storage_account_name
  assign_delivery_team_key_vault_permissions = var.assign_delivery_team_key_vault_permissions
  basic_auth_password                        = var.basic_auth_password
  email_support_address                      = var.email_support_address
  tags                                       = local.common_tags
  asp_sku_notification                       = var.asp_sku_notification
}
