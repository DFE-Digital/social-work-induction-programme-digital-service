module "web-app" {
  source = "./modules/web-app"

  environment                  = var.environment
  location                     = var.azure_region
  resource_group               = module.stack.resource_group_name
  resource_name_prefix         = var.resource_name_prefix
  web_app_name                 = var.webapp_name
  front_door_profile_web_id    = module.stack.front_door_profile_web_id
  subnet_webapps_id            = module.stack.subnet_webapps_id
  tags                         = local.common_tags

  app_settings = {
    "ENVIRONMENT"                         = var.environment
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
    "POSTGRES_DB"                         = var.moodle_db_name
    "POSTGRES_USER"                       = module.stack.postgres_username
    "POSTGRES_PASSWORD"                   = "@Microsoft.KeyVault(SecretUri=${stack.full_postgres_secret_uri})"
    "MOODLE_DB_TYPE"                      = var.moodle_db_type
    "MOODLE_DB_HOST"                      = module.stack.postgres_db_host
    "MOODLE_DB_PREFIX"                    = var.moodle_db_prefix
    "MOODLE_DOCKER_WEB_HOST"              = var.webapp_name
    "MOODLE_DOCKER_WEB_PORT"              = var.moodle_web_port
    "MOODLE_SITE_FULLNAME"                = var.moodle_site_fullname
    "MOODLE_SITE_SHORTNAME"               = var.moodle_site_shortname
    "MOODLE_ADMIN_USER"                   = var.moodle_admin_user
    "MOODLE_ADMIN_PASSWORD"               = var.moodle_admin_password
    "MOODLE_ADMIN_EMAIL"                  = var.moodle_admin_email
    DOCKER_ENABLE_CI                      = "true"
  }
}

resource "azurerm_postgresql_flexible_server_database" "moodle" {
  server_id = azurerm_postgresql_flexible_server.swipdb.id
  name      = var.moodle_db_name
}
