module "web_app_trace" {
  source                    = "./modules/web-app"
  tenant_id                 = data.azurerm_client_config.az_config.tenant_id
  environment               = var.environment
  location                  = var.azure_region
  resource_group            = module.stack.resource_group_name
  resource_name_prefix      = var.resource_name_prefix
  web_app_name              = "${var.resource_name_prefix}-webapp-trace"
  web_app_short_name        = "wa-trace"
  docker_image_name         = "dfe-digital/swip-digital-service-trace-app:latest"
  front_door_profile_web_id = module.stack.front_door_profile_web_id
  subnet_webapps_id         = module.stack.subnet_webapps_id
  acr_id                    = azurerm_container_registry.acr.id
  key_vault_id              = module.stack.kv_id
  service_plan_id           = module.stack.service_plan_id
  tags                      = local.common_tags

  app_settings = {
    "ENVIRONMENT"                         = var.environment
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
    "TRACE_WEBAPP_ADMIN_USER"             = var.trace_webapp_admin_user
    "TRACE_WEBAPP_ADMIN_PASSWORD"         = var.trace_webapp_admin_password
    DOCKER_ENABLE_CI                      = "true"
  }
}
