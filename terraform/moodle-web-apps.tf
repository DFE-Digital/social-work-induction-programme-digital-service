locals {
  moodle_webapp_name_stem    = "${var.resource_name_prefix}-wa-moodle"
  moodle_webapp_db_name_stem = "${var.resource_name_prefix}-db-wa-moodle"

  moodle_instance_resource_naming = {
    for instance in keys(var.moodle_instances) : instance => {
      webapp_name        = "${local.moodle_webapp_name_stem}-${instance}"
      web_app_short_name = "wa-moodle-${instance}"
      db_name            = "${local.moodle_webapp_db_name_stem}-${instance}"
    }
  }
}

resource "azurerm_postgresql_flexible_server_database" "moodle_db" {
  for_each  = local.moodle_instance_resource_naming
  server_id = module.stack.db_server_id
  name      = each.value.db_name
}

module "web_app_moodle" {
  for_each = local.moodle_instance_resource_naming

  source                    = "./modules/web-app"
  tenant_id                 = data.azurerm_client_config.az_config.tenant_id
  environment               = var.environment
  location                  = var.azure_region
  resource_group            = module.stack.resource_group_name
  resource_name_prefix      = var.resource_name_prefix
  web_app_name              = each.value.webapp_name
  web_app_short_name        = each.value.web_app_short_name
  docker_image_name         = "dfe-digital/nothing:latest"
  front_door_profile_web_id = module.stack.front_door_profile_web_id
  subnet_webapps_id         = module.stack.subnet_moodle_id
  acr_id                    = azurerm_container_registry.acr.id
  acr_name                  = var.acr_name
  key_vault_id              = module.stack.kv_id
  service_plan_id           = module.stack.moodle_service_plan_id
  tags                      = local.common_tags

  app_settings = {
    "ENVIRONMENT"                         = var.environment
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
    "POSTGRES_DB"                         = each.value.db_name
    "POSTGRES_USER"                       = module.stack.postgres_username
    "POSTGRES_PASSWORD"                   = "@Microsoft.KeyVault(SecretUri=${module.stack.full_postgres_secret_password_uri})"
    "MOODLE_DB_TYPE"                      = var.moodle_db_type
    "MOODLE_DB_HOST"                      = module.stack.postgres_db_host
    "MOODLE_DB_PREFIX"                    = var.moodle_db_prefix
    "MOODLE_DOCKER_WEB_HOST"              = "${each.value.webapp_name}.azurewebsites.net"
    "MOODLE_DOCKER_SSL_TERMINATION"       = "true"
    "MOODLE_SITE_FULLNAME"                = var.moodle_site_fullname
    "MOODLE_SITE_SHORTNAME"               = var.moodle_site_shortname
    "MOODLE_ADMIN_USER"                   = "${var.moodle_admin_user}"
    "MOODLE_ADMIN_PASSWORD"               = var.moodle_admin_password
    "MOODLE_ADMIN_EMAIL"                  = var.moodle_admin_email
    DOCKER_ENABLE_CI                      = "false" # Github will control CI, not Azure
  }

  depends_on = [
    azurerm_postgresql_flexible_server_database.moodle_db
  ]
}

# One install webapp handles multiple moodle instances in the same logical environment
module "web_app_moodle_install" {
  source                    = "./modules/web-app"
  tenant_id                 = data.azurerm_client_config.az_config.tenant_id
  environment               = var.environment
  location                  = var.azure_region
  resource_group            = module.stack.resource_group_name
  resource_name_prefix      = var.resource_name_prefix
  web_app_name              = "${local.moodle_webapp_name_stem}-install"
  web_app_short_name        = "wa-moodle-install"
  docker_image_name         = "dfe-digital/nothing:latest"
  front_door_profile_web_id = module.stack.front_door_profile_web_id
  subnet_webapps_id         = module.stack.subnet_maintenance_id
  acr_id                    = azurerm_container_registry.acr.id
  acr_name                  = var.acr_name
  key_vault_id              = module.stack.kv_id
  service_plan_id           = module.stack.maintenance_service_plan_id
  tags                      = local.common_tags

  # POSTGRES_DB should be changed when deploying the installation webapp.
  # This is because one installation webapp can service multiple moodle
  # instances, the only difference being the database name.

  app_settings = {
    "ENVIRONMENT"                         = var.environment
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
    "POSTGRES_DB"                         = "${local.moodle_webapp_db_name_stem}-primary"
    "POSTGRES_USER"                       = module.stack.postgres_username
    "POSTGRES_PASSWORD"                   = "@Microsoft.KeyVault(SecretUri=${module.stack.full_postgres_secret_password_uri})"
    "MOODLE_DB_TYPE"                      = var.moodle_db_type
    "MOODLE_DB_HOST"                      = module.stack.postgres_db_host
    "MOODLE_DB_PREFIX"                    = var.moodle_db_prefix
    "MOODLE_DOCKER_WEB_HOST"              = "${local.moodle_webapp_name_stem}-install.azurewebsites.net"
    "MOODLE_DOCKER_SSL_TERMINATION"       = "true"
    "MOODLE_SITE_FULLNAME"                = var.moodle_site_fullname
    "MOODLE_SITE_SHORTNAME"               = var.moodle_site_shortname
    "MOODLE_ADMIN_USER"                   = "${var.moodle_admin_user}"
    "MOODLE_ADMIN_PASSWORD"               = var.moodle_admin_password
    "MOODLE_ADMIN_EMAIL"                  = var.moodle_admin_email
    DOCKER_ENABLE_CI                      = "false" # Github will control CI, not Azure
  }

  depends_on = [
    azurerm_postgresql_flexible_server_database.moodle_db
  ]
}
