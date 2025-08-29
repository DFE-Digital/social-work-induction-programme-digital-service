module "db-jobs-app" {
  source                                = "./modules/function-app"
  environment                           = var.environment
  location                              = var.azure_region
  resource_group                        = module.stack.resource_group_name
  resource_name_prefix                  = var.resource_name_prefix
  function_app_name                     = "${var.resource_name_prefix}-fa-db-jobs"
  acr_name                              = var.acr_name
  acr_id                                = local.acr_id
  service_plan_id                       = module.stack.db_jobs_service_plan_id
  tags                                  = local.common_tags
  docker_image_name                     = "dfe-digital/nothing:latest"
  storage_account_name                  = module.stack.file_storage_account_name
  storage_account_access_key            = module.stack.file_storage_access_key
  appinsights_connection_string         = module.stack.appinsights_connection_string
  health_check_path                     = "/api/health"
  subnet_functionapp_id                 = module.stack.subnet_functionapp_id
  virtual_network_name                  = module.stack.vnet_name
  virtual_network_id                    = module.stack.vnet_id
  key_vault_id                          = module.stack.kv_id
  function_worker_runtime               = "dotnet-isolated"
  subnet_functionapp_privateendpoint_id = module.stack.subnet_functionapp_privateendpoint_id
  private_dns_zone_id                   = module.stack.private_dns_zone_id
  app_settings = merge({
    "CONNECTIONSTRINGS__DEFAULTCONNECTION" = "Host=${module.stack.postgres_db_host};Username=${module.stack.postgres_username};"
    "STORAGECONNECTIONSTRING"              = "@Microsoft.KeyVault(SecretUri=${module.stack.full_backup_storage_connectionstring_uri})"
    "DB_PASSWORD"                          = "@Microsoft.KeyVault(SecretUri=${module.stack.full_postgres_secret_password_uri})"
  }, var.db_jobs_app_settings)
}

# Front Door route for this function app only
resource "azurerm_cdn_frontdoor_endpoint" "fd_endpoint" {
  name                     = "fd-ops-trigger-${random_string.suffix.result}"
  cdn_frontdoor_profile_id = module.stack.front_door_profile_web_id
  tags                     = local.common_tags

  lifecycle {
    ignore_changes = [tags]
  }
}

resource "azurerm_cdn_frontdoor_origin_group" "origin_group" {
  name                     = "${var.resource_name_prefix}-fd-origin-group-web-fa-db-operations"
  cdn_frontdoor_profile_id = module.stack.front_door_profile_web_id

  health_probe {
    path                = "/api/health"
    protocol            = "Https"
    interval_in_seconds = 60
  }

  load_balancing {
    sample_size                 = 4
    successful_samples_required = 2
  }
}

resource "azurerm_cdn_frontdoor_origin" "origin" {
  cdn_frontdoor_origin_group_id  = azurerm_cdn_frontdoor_origin_group.origin_group.id
  enabled                        = true
  certificate_name_check_enabled = false
  host_name                      = module.function_app.function_app_default_hostname
  http_port                      = 80
  https_port                     = 443
  origin_host_header             = module.function_app.function_app_default_hostname
  priority                       = 1
  weight                         = 1
  name                           = "${var.resource_name_prefix}-fd-origin-web-fa-db-operations"
}

resource "azurerm_cdn_frontdoor_route" "route" {
  name                          = "${var.resource_name_prefix}-fd-route-web-fa-db-operations"
  cdn_frontdoor_endpoint_id     = azurerm_cdn_frontdoor_endpoint.front_door_endpoint_web.id
  cdn_frontdoor_origin_group_id = azurerm_cdn_frontdoor_origin_group.front_door_origin_group_web.id
  cdn_frontdoor_origin_ids      = [azurerm_cdn_frontdoor_origin.front_door_origin_web.id]

  forwarding_protocol    = "MatchRequest"
  https_redirect_enabled = true
  patterns_to_match      = ["/*"]
  supported_protocols    = ["Http", "Https"]
}
