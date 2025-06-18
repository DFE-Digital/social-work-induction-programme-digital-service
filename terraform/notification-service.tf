module "notification-service" {
  source               = "./modules/function-app"
  tenant_id            = data.azurerm_client_config.az_config.tenant_id
  environment          = var.environment
  location             = var.azure_region
  resource_group       = module.stack.resource_group_name
  resource_name_prefix = var.resource_name_prefix
  function_app_name    = "${var.resource_name_prefix}-wa-notification-service"
  acr_id               = local.acr_id
  acr_name             = var.acr_name
  service_plan_id      = module.stack.notification_service_plan_id
  tags                 = local.common_tags
}
