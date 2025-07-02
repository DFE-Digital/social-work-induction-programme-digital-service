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
  storage_account_name          = module.stack.storage_account_name
  storage_account_access_key    = module.stack.storage_account_access_key
  appinsights_connection_string = module.stack.appinsights_connection_string
  health_check_path             = "/api/health"
  subnet_functionapp_id         = subnet.stack.subnet_functionapp_id
}
