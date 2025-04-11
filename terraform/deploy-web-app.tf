resource "tls_private_key" "ssh_key" {
  algorithm = "RSA"
  rsa_bits  = 4096
}

resource "azurerm_key_vault_secret" "deploy_ssh_cert" {
  name         = "Deploy--SshKey"
  value        = base64encode(tls_private_key.ssh_key.private_key_pem)
  key_vault_id = module.stack.kv_id
  content_type = "ssh key"
}

module "web_app_deploy" {
  source                    = "./modules/web-app"
  tenant_id                 = data.azurerm_client_config.az_config.tenant_id
  environment               = var.environment
  location                  = var.azure_region
  resource_group            = module.stack.resource_group_name
  resource_name_prefix      = var.resource_name_prefix
  web_app_name              = "${var.resource_name_prefix}-webapp-deploy"
  web_app_short_name        = "wa-deploy"
  docker_image_name         = "dfe-digital/swip-digital-service-deploy-app:latest"
  front_door_profile_web_id = module.stack.front_door_profile_web_id
  subnet_webapps_id         = module.stack.subnet_webapps_id
  acr_id                    = azurerm_container_registry.acr.id
  key_vault_id              = module.stack.kv_id
  service_plan_id           = module.stack.service_plan_id
  tags                      = local.common_tags

  app_settings = {
    "ENVIRONMENT"                         = var.environment
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
    "SSH_CERT_BASE64"                     = "@Microsoft.KeyVault(SecretUri=${module.stack.kv_vault_uri}secrets/${azurerm_key_vault_secret.deploy_ssh_cert.name})"
    "POSTGRES_USER"                       = module.stack.postgres_username
    "POSTGRES_PASSWORD"                   = "@Microsoft.KeyVault(SecretUri=${module.stack.full_postgres_secret_password_uri})"
    DOCKER_ENABLE_CI                      = "false" # Github will control CI, not Azure
  }
}
