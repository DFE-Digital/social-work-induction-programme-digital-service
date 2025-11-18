locals {
  auth_service_web_app_name    = "${var.resource_name_prefix}-wa-auth-service"
  auth_service_client_id       = var.auth_service_client_id
  auth_service_client_secret   = "@Microsoft.KeyVault(SecretUri=${module.stack.kv_vault_uri}secrets/${azurerm_key_vault_secret.auth_service_client_secret.name})"
  auth_service_end_point       = "${module.auth_service.front_door_app_url}/oauth2/authorize"
  auth_service_token_end_point = "${module.auth_service.front_door_app_url}/oauth2/token"
  auth_service_logout_uri      = "${module.auth_service.front_door_app_url}/oauth2/logout"
}

resource "azurerm_postgresql_flexible_server_database" "auth_db" {
  server_id = module.stack.db_server_id
  name      = "${var.resource_name_prefix}_db_auth_service"
}

resource "random_password" "auth_service_client_secret" {
  length           = 50
  special          = true
  override_special = "!#$%&*-_=+<>:?"
}

resource "time_offset" "secret_expiry01" {
  offset_days = 365
}

resource "azurerm_key_vault_secret" "auth_service_client_secret" {
  name            = "AuthService-ClientSecret"
  value           = random_password.auth_service_client_secret.result
  key_vault_id    = module.stack.kv_id
  content_type    = "client secret"
  expiration_date = time_offset.secret_expiry01.rfc3339

  lifecycle {
    ignore_changes = [value]
  }
}

module "signing_certificate" {
  source       = "./modules/certificate"
  key_vault_id = module.stack.kv_id
  cert_name    = "AuthService-OpenIddictSigningCert"
}

module "encryption_certificate" {
  source       = "./modules/certificate"
  key_vault_id = module.stack.kv_id
  cert_name    = "AuthService-OpenIddictEncryptionCert"
}

module "auth_service" {
  source                    = "./modules/web-app"
  tenant_id                 = data.azurerm_client_config.az_config.tenant_id
  environment               = var.environment
  location                  = var.azure_region
  resource_group            = module.stack.resource_group_name
  resource_name_prefix      = var.resource_name_prefix
  web_app_name              = local.auth_service_web_app_name
  web_app_short_name        = "wa-auth-service"
  docker_image_name         = "dfe-digital/nothing:latest"
  front_door_profile_web_id = module.stack.front_door_profile_web_id
  subnet_webapps_id         = module.stack.subnet_services_id
  allow_subnet_ids          = [module.stack.subnet_services_id]
  acr_id                    = local.acr_id
  acr_name                  = var.acr_name
  key_vault_id              = module.stack.kv_id
  service_plan_id           = module.stack.services_service_plan_id
  health_check_path         = "/api/accounts/version"
  support_action_group_id   = module.stack.support_action_group_id
  tags                      = local.common_tags

  # The settings name syntax below (e.g. OIDC__ISSUER) is how .NET imports environment 
  # variables to override the properties in its multi-level appsettings.json file
  app_settings = merge({
    "ENVIRONMENT"                                      = var.environment
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE"              = "false"
    "CONNECTIONSTRINGS__DEFAULTCONNECTION"             = "Host=${module.stack.postgres_db_host};Database=${azurerm_postgresql_flexible_server_database.auth_db.name};Username=${module.stack.postgres_username};Password=$[DB_REPLACE_PASSWORD];Ssl Mode=Require;Trust Server Certificate=false"
    "STORAGECONNECTIONSTRING"                          = "https://${module.stack.blob_storage_account_name}.blob.core.windows.net/${azurerm_storage_container.dpkeys.name}/dpkeys"
    "DB_SERVER_NAME"                                   = module.stack.postgres_db_host
    "DB_DATABASE_NAME"                                 = azurerm_postgresql_flexible_server_database.auth_db.name
    "DB_USER_NAME"                                     = module.stack.postgres_username
    "DB_PASSWORD"                                      = "@Microsoft.KeyVault(SecretUri=${module.stack.full_postgres_secret_password_uri})"
    "KEYVAULTURI"                                      = module.stack.kv_vault_uri
    "OIDC__ISSUER"                                     = "https://${local.auth_service_web_app_name}.azurewebsites.net"
    "OIDC__SIGNINGCERTIFICATENAME"                     = module.signing_certificate.cert_name
    "OIDC__ENCRYPTIONCERTIFICATENAME"                  = module.encryption_certificate.cert_name
    "OIDC__APPLICATIONS__0__CLIENTID"                  = local.auth_service_client_id
    "OIDC__APPLICATIONS__0__CLIENTSECRET"              = local.auth_service_client_secret
    "OIDC__APPLICATIONS__0__REDIRECTURIS__0"           = local.moodle_auth_redirect_uri
    "OIDC__APPLICATIONS__0__POSTLOGOUTREDIRECTURIS__0" = local.moodle_auth_post_logout_redirect_uri
    "OIDC__APPLICATIONS__0__REDIRECTURIS__1"           = local.user_management_auth_redirect_uri
    "OIDC__APPLICATIONS__0__POSTLOGOUTREDIRECTURIS__1" = local.user_management_auth_post_logout_redirect_uri
    "ONELOGIN__CLIENTID"                               = var.one_login_client_id
    "ONELOGIN__CERTIFICATENAME"                        = module.signing_certificate.cert_name
    "ONELOGIN__URL"                                    = var.one_login_oidc_url
    "APPLICATIONINSIGHTS_CONNECTION_STRING"            = module.stack.appinsights_connection_string
    DOCKER_ENABLE_CI                                   = "false" # Github will control CI, not Azure
  }, var.auth_service_app_settings)

  depends_on = [
    azurerm_postgresql_flexible_server_database.auth_db
  ]
}

resource "azurerm_storage_container" "dpkeys" {
  name               = "${var.resource_name_prefix}-sc-auth-service-dpkeys"
  storage_account_id = module.stack.blob_storage_account_id

  # Prevent any anonymous or public blob reads
  container_access_type = "private"
}

resource "azurerm_role_assignment" "dpkeys" {
  scope                = azurerm_storage_container.dpkeys.id
  role_definition_name = "Storage Blob Data Contributor"
  principal_type       = "ServicePrincipal"
  principal_id         = module.auth_service.web_app_id
}
