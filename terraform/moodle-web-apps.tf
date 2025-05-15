locals {
  moodle_webapp_name_stem    = "${var.resource_name_prefix}-wa-moodle"
  moodle_webapp_db_name_stem = "${var.resource_name_prefix}_db_moodle"

  moodle_instance_resource_naming = {
    for instance in keys(var.moodle_instances) : instance => {
      webapp_name        = "${local.moodle_webapp_name_stem}-${instance}"
      web_app_short_name = "wa-moodle-${instance}"
      db_name            = "${local.moodle_webapp_db_name_stem}_${instance}"
    }
  }
  moodle_shared_app_settings = {
    "ENVIRONMENT"                         = var.environment
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
    "POSTGRES_DB"                         = "${local.moodle_webapp_db_name_stem}_primary"
    "POSTGRES_USER"                       = module.stack.postgres_username
    "POSTGRES_PASSWORD"                   = "@Microsoft.KeyVault(SecretUri=${module.stack.full_postgres_secret_password_uri})"
    "MOODLE_DB_TYPE"                      = var.moodle_db_type
    "MOODLE_DB_HOST"                      = module.stack.postgres_db_host
    "MOODLE_DB_PREFIX"                    = var.moodle_db_prefix
    "MOODLE_DOCKER_WEB_HOST"              = "${local.moodle_webapp_name_stem}-primary.azurewebsites.net"
    "MOODLE_DOCKER_SSL_TERMINATION"       = "true"
    "MOODLE_SITE_FULLNAME"                = var.moodle_site_fullname
    "MOODLE_SITE_SHORTNAME"               = var.moodle_site_shortname
    "MOODLE_ADMIN_USER"                   = var.moodle_admin_user
    "MOODLE_ADMIN_PASSWORD"               = var.moodle_admin_password
    "MOODLE_ADMIN_EMAIL"                  = var.moodle_admin_email
    DOCKER_ENABLE_CI                      = "false" # Github will control CI, not Azure
  }
  moodle_web_app_oidc_url = "${module.web_app_moodle["primary"].front_door_app_url}/auth/oidc/"
}

resource "random_password" "web_service_user_password" {
  length           = 25
  special          = true
  override_special = "!#$%&*-_=+<>:?"
}

resource "azurerm_key_vault_secret" "web_service_user_password" {
  name         = "Moodle-WebServicePassword"
  value        = random_password.web_service_user_password.result
  key_vault_id = module.stack.kv_id
  content_type = "password"

  // TODO: Managed expiry of passwords
  //expiration_date = local.expiration_date

  lifecycle {
    ignore_changes = [value, expiration_date]
  }

  depends_on = [module.stack]

  #checkov:skip=CKV_AZURE_41:Ensure that the expiration date is set on all secrets
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
  acr_id                    = local.acr_id
  acr_name                  = var.acr_name
  key_vault_id              = module.stack.kv_id
  service_plan_id           = module.stack.moodle_service_plan_id
  tags                      = local.common_tags

  app_settings = merge({
    "IS_CRON_JOB_ONLY"                 = "false"
    "MOODLE_WEB_SERVICE_NAME"          = var.moodle_web_service_name
    "MOODLE_WEB_SERVICE_USER"          = var.moodle_web_service_user
    "MOODLE_WEB_SERVICE_USER_EMAIL"    = var.moodle_web_service_user_email
    "MOODLE_WEB_SERVICE_USER_PASSWORD" = "@Microsoft.KeyVault(SecretUri=${module.stack.kv_vault_uri}secrets/${azurerm_key_vault_secret.web_service_user_password.name})"
    "AUTH_SERVICE_CLIENT_ID"           = local.auth_service_client_id
    "AUTH_SERVICE_CLIENT_SECRET"       = local.auth_service_client_secret
    "AUTH_SERVICE_END_POINT"           = local.auth_service_end_point
    "AUTH_SERVICE_TOKEN_END_POINT"     = local.auth_service_token_end_point
    "AUTH_SERVICE_LOGOUT_URI"          = local.auth_service_logout_uri
  }, var.moodle_app_settings, local.moodle_shared_app_settings)

  depends_on = [
    azurerm_postgresql_flexible_server_database.moodle_db
  ]
}

# The cron app service works just for the primary Moodle instance
module "web_app_moodle_cron" {
  source                    = "./modules/web-app"
  tenant_id                 = data.azurerm_client_config.az_config.tenant_id
  environment               = var.environment
  location                  = var.azure_region
  resource_group            = module.stack.resource_group_name
  resource_name_prefix      = var.resource_name_prefix
  web_app_name              = "${local.moodle_webapp_name_stem}-cron"
  web_app_short_name        = "wa-moodle-cron"
  docker_image_name         = "dfe-digital/nothing:latest"
  front_door_profile_web_id = module.stack.front_door_profile_web_id
  subnet_webapps_id         = module.stack.subnet_maintenance_id
  acr_id                    = local.acr_id
  acr_name                  = var.acr_name
  key_vault_id              = module.stack.kv_id
  service_plan_id           = module.stack.maintenance_service_plan_id
  tags                      = local.common_tags

  # POSTGRES_DB should be changed when deploying the installation webapp.
  # This is because one installation webapp can service multiple moodle
  # instances, the only difference being the database name.

  app_settings = merge({
    "IS_CRON_JOB_ONLY" = "true"
  }, local.moodle_shared_app_settings)

  depends_on = [
    azurerm_postgresql_flexible_server_database.moodle_db
  ]
}
