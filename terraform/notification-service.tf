resource "azurerm_key_vault_secret" "govnotify_api_key" {
  name         = "NotificationService-GovNotifyApiKey"
  value        = var.govnotify_api_key
  key_vault_id = module.stack.kv_id
  content_type = "API Key"

  lifecycle {
    ignore_changes = [value]
  }
  #checkov:skip=CKV_AZURE_41:No expiry date
}

resource "azurerm_key_vault_secret" "function_key" {
  name         = "NotificationService-FunctionKey"
  value        = module.notification-service.function_app_key
  key_vault_id = module.stack.kv_id
  content_type = "function app key"
  #checkov:skip=CKV_AZURE_41:No expiry date
}

module "notification-service" {
  source                        = "./modules/function-app"
  environment                   = var.environment
  location                      = var.azure_region
  resource_group                = module.stack.resource_group_name
  resource_name_prefix          = var.resource_name_prefix
  function_app_name             = "${var.resource_name_prefix}-fa-notification-service"
  acr_name                      = var.acr_name
  acr_id                        = local.acr_id
  service_plan_id               = module.stack.notification_service_plan_id
  tags                          = local.common_tags
  docker_image_name             = "dfe-digital/nothing:latest"
  storage_account_name          = module.stack.file_storage_account_name
  storage_account_access_key    = module.stack.file_storage_access_key
  appinsights_connection_string = module.stack.appinsights_connection_string
  health_check_path             = "/api/health"
  subnet_functionapp_id         = module.stack.subnet_functionapp_id
  virtual_network_name          = module.stack.vnet_name
  virtual_network_id            = module.stack.vnet_id
  key_vault_id                  = module.stack.kv_id

  app_settings = merge({
    "GOVNOTIFY__APIKEY" = "@Microsoft.KeyVault(SecretUri=${module.stack.kv_vault_uri}secrets/${azurerm_key_vault_secret.govnotify_api_key.name})"
  }, var.notification_service_app_settings)
}
