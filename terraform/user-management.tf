locals {
  user_management_auth_redirect_uri             = "${module.user_management.front_door_app_url}/oidc/callback"
  user_management_auth_post_logout_redirect_uri = "${module.user_management.front_door_app_url}/oidc/logout-callback"
}

module "user_management" {
  source                    = "./modules/web-app"
  tenant_id                 = data.azurerm_client_config.az_config.tenant_id
  environment               = var.environment
  location                  = var.azure_region
  resource_group            = module.stack.resource_group_name
  resource_name_prefix      = var.resource_name_prefix
  web_app_name              = "${var.resource_name_prefix}-wa-user-management"
  web_app_short_name        = "wa-user-management"
  docker_image_name         = "dfe-digital/nothing:latest"
  front_door_profile_web_id = module.stack.front_door_profile_web_id
  subnet_webapps_id         = module.stack.subnet_services_id
  acr_id                    = local.acr_id
  acr_name                  = var.acr_name
  key_vault_id              = module.stack.kv_id
  service_plan_id           = module.stack.services_service_plan_id
  health_check_path         = "/version.txt"
  support_action_group_id   = module.stack.support_action_group_id
  tags                      = local.common_tags

  # The settings name syntax below (e.g. OIDC__AUTHORITYURL) is how .NET imports environment 
  # variables to override the properties in its multi-level appsettings.json file
  #
  app_settings = merge({
    "ENVIRONMENT"                             = var.environment
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE"     = "false"
    "OIDC__AUTHORITYURL"                      = module.auth_service.front_door_app_url
    "OIDC__CLIENTID"                          = var.auth_service_client_id
    "OIDC__CLIENTSECRET"                      = "@Microsoft.KeyVault(SecretUri=${module.stack.kv_vault_uri}secrets/${azurerm_key_vault_secret.auth_service_client_secret.name})"
    "SOCIALWORKENGLANDCLIENTOPTIONS__BASEURL" = "TBD" # TODO: SWE API usage deprioritised
    "AUTHCLIENTOPTIONS__BASEURL"              = module.auth_service.front_door_app_url
    "MOODLECLIENTOPTIONS__BASEURL"            = "${module.web_app_moodle["primary"].front_door_app_url}/webservice/rest/server.php"
    "BASIC_AUTH_USER"                         = var.basic_auth_user
    "BASIC_AUTH_PASSWORD"                     = "@Microsoft.KeyVault(SecretUri=${module.stack.kv_vault_uri}secrets/Sites-BasicAuthPassword)"
    DOCKER_ENABLE_CI                          = "false" # Github will control CI, not Azure
  }, var.user_management_app_settings)
}
