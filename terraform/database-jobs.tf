module "database-jobs" {
  source                     = "./modules/container-app"
  vnet_subnet_id             = module.stack.subnet_containerapps_id
  container_app_job_name     = "${var.resource_name_prefix}-database-jobs-ca"
  location                   = var.azure_region
  key_vault_id               = module.stack.kv_id
  storage_account_id         = module.stack.db_backup_blob_Storage_account_id
  container_image            = "mcr.microsoft.com/cbl-mariner/base/core:2.0" # container app jobs require an image, will replace this with a custom image in CI/CD later
  log_analytics_workspace_id = module.stack.log_analytics_workspace_id
  resource_group_name        = var.primary_resource_group
  tags                       = local.common_tags
  acr_id                     = local.acr_id
  container_app_env_id       = module.stack.container_env_id
  environment_settings = merge({
    "CONNECTIONSTRINGS__DEFAULTCONNECTION" = "Host=${module.stack.postgres_db_host};Username=${module.stack.postgres_username};"
    "STORAGECONNECTIONSTRING"              = "https://${module.stack.db_backup_blob_storage_account_name}.blob.core.windows.net/[SA_REPLACE_CONTAINER]"
    "DB_PASSWORD"                          = "@Microsoft.KeyVault(SecretUri=${module.stack.full_postgres_secret_password_uri})"
  })
}
