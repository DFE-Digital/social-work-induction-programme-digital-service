resource "azurerm_postgresql_flexible_server_database" "auth_db" {
  server_id = module.stack.db_server_id
  name      = "${var.resource_name_prefix}-db-wa-auth-service"
}

# One install webapp handles multiple moodle instances in the same logical environment
module "auth_service_moodle_install" {
  source                    = "./modules/web-app"
  tenant_id                 = data.azurerm_client_config.az_config.tenant_id
  environment               = var.environment
  location                  = var.azure_region
  resource_group            = module.stack.resource_group_name
  resource_name_prefix      = var.resource_name_prefix
  web_app_name              = "${var.resource_name_prefix}-wa-auth-service"
  web_app_short_name        = "wa-auth-service"
  docker_image_name         = "dfe-digital/nothing:latest"
  front_door_profile_web_id = module.stack.front_door_profile_web_id
  subnet_webapps_id         = module.stack.subnet_services_id
  acr_id                    = azurerm_container_registry.acr.id
  acr_name                  = var.acr_name
  key_vault_id              = module.stack.kv_id
  service_plan_id           = module.stack.services_service_plan_id
  tags                      = local.common_tags

  app_settings = {
    "ENVIRONMENT"                         = var.environment
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
    "POSTGRES_DB_HOST"                    = module.stack.postgres_db_host
    "POSTGRES_DB"                         = azurerm_postgresql_flexible_server_database.auth_db.name
    "POSTGRES_USER"                       = module.stack.postgres_username
    "POSTGRES_PASSWORD"                   = "@Microsoft.KeyVault(SecretUri=${module.stack.full_postgres_secret_password_uri})"
    DOCKER_ENABLE_CI                      = "false" # Github will control CI, not Azure
  }

  depends_on = [
    azurerm_postgresql_flexible_server_database.moodle_db
  ]
}
