module "stack" {
  source = "./modules/environment-stack"

  environment                                = var.environment
  location                                   = var.azure_region
  primary_resource_group                     = var.primary_resource_group
  resource_name_prefix                       = var.resource_name_prefix
  kv_purge_protection_enabled                = var.kv_purge_protection_enabled
  asp_sku_moodle                             = var.asp_sku_moodle
  asp_sku_maintenance                        = var.asp_sku_maintenance
  asp_sku_services                           = var.asp_sku_services
  admin_enabled                              = var.admin_enabled
  webapp_storage_account_name                = var.webapp_storage_account_name
  assign_delivery_team_key_vault_permissions = var.assign_delivery_team_key_vault_permissions
  basic_auth_password                        = local.basic_auth_password
  email_support_address                      = var.email_support_address
  tags                                       = local.common_tags
  asp_sku_notification                       = var.asp_sku_notification
  postgresql_sku                             = var.postgresql_sku
  frontdoor_sku                              = var.frontdoor_sku
  log_analytics_sku                          = var.log_analytics_sku
  key_vault_sku                              = var.key_vault_sku
  db_backup_blob_sa_name                     = var.db_backup_blob_sa_name
  container_app_env_name                     = var.container_app_env_name
}
