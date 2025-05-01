module "proxy_service" {
  source                    = "./modules/web-app"
  tenant_id                 = data.azurerm_client_config.az_config.tenant_id
  environment               = var.environment
  location                  = var.azure_region
  resource_group            = module.stack.resource_group_name
  resource_name_prefix      = var.resource_name_prefix
  web_app_name              = "${var.resource_name_prefix}-wa-proxy-service"
  web_app_short_name        = "proxy-service"
  docker_image_name         = "dfe-digital/nothing:latest"
  front_door_profile_web_id = module.stack.front_door_profile_web_id
  subnet_webapps_id         = module.stack.subnet_services_id
  acr_id                    = local.acr_id
  acr_name                  = var.acr_name
  key_vault_id              = module.stack.kv_id
  service_plan_id           = module.stack.maintenance_service_plan_id
  tags                      = local.common_tags

  app_settings = {
    "ENVIRONMENT"                          = var.environment
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE"  = "false"
    "CONNECTIONSTRINGS__DEFAULTCONNECTION" = "Host=${module.stack.postgres_db_host};Database=$[DB_REPLACE_DB_NAME];Username=${module.stack.postgres_username};Password=$[DB_REPLACE_PASSWORD];Ssl Mode=Require;Trust Server Certificate=false"
    "DB_PASSWORD"                          = "@Microsoft.KeyVault(SecretUri=${module.stack.full_postgres_secret_password_uri})"
    DOCKER_ENABLE_CI                       = "false" # Github will control CI, not Azure
  }
}
