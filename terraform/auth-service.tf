resource "tls_private_key" "keypair" {
  algorithm = "RSA"
  rsa_bits  = 2048
}

locals {
  auth_service_web_app_name = "${var.resource_name_prefix}-wa-auth-service"
  moodle_web_app_oidc_url   = "${module.web_app_moodle["primary"].web_app_url}/auth/oidc/"
  private_key_single_line   = replace(tls_private_key.keypair.private_key_pem, "\n", "")
}

resource "azurerm_postgresql_flexible_server_database" "auth_db" {
  server_id = module.stack.db_server_id
  name      = "${var.resource_name_prefix}_db_auth_service"
}

resource "azurerm_key_vault_secret" "auth_service_client_secret" {
  name         = "AuthService-ClientSecret"
  value        = var.auth_service_client_secret
  key_vault_id = module.stack.kv_id
  content_type = "client secret"

  #checkov:skip=CKV_AZURE_41:No expiry date
}

resource "azurerm_key_vault_secret" "one_login_private_key_pem" {
  name         = "AuthService-OneLoginPrivateKeyPem"
  key_vault_id = module.stack.kv_id
  value        = local.private_key_single_line
  content_type = "application/x-pem-file"

  #checkov:skip=CKV_AZURE_41:No expiry date
}

# The OneLogin public key is entered manually into the admin site 
# (https://admin.sign-in.service.gov.uk/) for non-prod environments. The admin site will 
# generate a client ID in return. For prod the public key must be given to the central 
# OneLogin team for configuration. They will send you the client ID after the prod 
# instance has been configured.

resource "azurerm_key_vault_secret" "one_login_public_key_pem" {
  name         = "AuthService-OneLoginPublicKeyPem"
  key_vault_id = module.stack.kv_id
  value        = tls_private_key.keypair.public_key_pem
  content_type = "application/x-pem-file"

  #checkov:skip=CKV_AZURE_41:No expiry date
}

module "signing_certificate" {
  source       = "./modules/certificate"
  key_vault_id = module.stack.kv_id
  cert_name    = "OpenIddictSigningCert"
}

module "encryption_certificate" {
  source       = "./modules/certificate"
  key_vault_id = module.stack.kv_id
  cert_name    = "OpenIddictEncryptionCert"
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
  acr_id                    = local.acr_id
  acr_name                  = var.acr_name
  key_vault_id              = module.stack.kv_id
  service_plan_id           = module.stack.services_service_plan_id
  tags                      = local.common_tags

  # The settings name syntax below (e.g. OIDC__ISSUER) is how .NET imports environment 
  # variables to override the properties in its multi-level appsettings.json file
  app_settings = merge({
    "ENVIRONMENT"                                      = var.environment
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE"              = "false"
    "CONNECTIONSTRINGS__DEFAULTCONNECTION"             = "Host=${module.stack.postgres_db_host};Database=${azurerm_postgresql_flexible_server_database.auth_db.name};Username=${module.stack.postgres_username};Password=$[DB_REPLACE_PASSWORD];Ssl Mode=Require;Trust Server Certificate=false"
    "STORAGECONNECTIONSTRING"                          = "https://${module.stack.storage_account_name}.blob.core.windows.net/${azurerm_storage_container.dpkeys.name}/dpkeys"
    "DB_SERVER_NAME"                                   = module.stack.postgres_db_host
    "DB_DATABASE_NAME"                                 = azurerm_postgresql_flexible_server_database.auth_db.name
    "DB_USER_NAME"                                     = module.stack.postgres_username
    "DB_PASSWORD"                                      = "@Microsoft.KeyVault(SecretUri=${module.stack.full_postgres_secret_password_uri})"
    "KEYVAULTURI"                                      = module.stack.kv_vault_uri
    "OIDC__ISSUER"                                     = "https://${local.auth_service_web_app_name}.azurewebsites.net"
    "OIDC__SIGNINGCERTIFICATENAME"                     = module.signing_certificate.cert_name
    "OIDC__ENCRYPTIONCERTIFICATENAME"                  = module.encryption_certificate.cert_name
    "OIDC__APPLICATIONS__0__CLIENTID"                  = var.auth_service_client_id
    "OIDC__APPLICATIONS__0__CLIENTSECRET"              = "@Microsoft.KeyVault(SecretUri=${module.stack.kv_vault_uri}secrets/${azurerm_key_vault_secret.auth_service_client_secret.name})"
    "OIDC__APPLICATIONS__0__REDIRECTURIS__0"           = local.moodle_web_app_oidc_url
    "OIDC__APPLICATIONS__0__POSTLOGOUTREDIRECTURIS__0" = "${local.moodle_web_app_oidc_url}logout.php"
    "ONELOGIN__CLIENTID"                               = var.one_login_client_id
    "ONELOGIN__PRIVATEKEYPEM"                          = "@Microsoft.KeyVault(SecretUri=${module.stack.kv_vault_uri}secrets/${azurerm_key_vault_secret.one_login_private_key_pem.name})"
    "ONELOGIN__URL"                                    = var.one_login_oidc_url
    DOCKER_ENABLE_CI                                   = "false" # Github will control CI, not Azure
  }, var.auth_service_feature_flag_overrides)

  depends_on = [
    azurerm_postgresql_flexible_server_database.auth_db
  ]
}

resource "azurerm_storage_container" "dpkeys" {
  name               = "${var.resource_name_prefix}-sc-auth-service-dpkeys"
  storage_account_id = module.stack.storage_account_id

  # Prevent any anonymous or public blob reads
  container_access_type = "private"
}

resource "azurerm_role_assignment" "dpkeys" {
  scope                = azurerm_storage_container.dpkeys.id
  role_definition_name = "Storage Blob Data Contributor"
  principal_type       = "ServicePrincipal"
  principal_id         = module.auth_service.web_app_id
}
