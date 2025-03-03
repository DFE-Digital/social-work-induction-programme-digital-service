# Create Resource Group
resource "azurerm_resource_group" "rg" {
  name     = "${var.resource_name_prefix}-rg-webapp"
  location = var.azure_region

  tags = local.common_tags

  lifecycle {
    ignore_changes = [
      tags["Environment"],
      tags["Product"],
      tags["Service Offering"]
    ]
  }
}

module "network" {
  source = "./modules/azure-network"

  environment          = var.environment
  location             = var.azure_region
  resource_group       = azurerm_resource_group.rg.name
  resource_name_prefix = var.resource_name_prefix
}

# Create storage account for web app
module "storage" {
  source = "./modules/azure-storage"

  location                    = var.azure_region
  resource_group              = azurerm_resource_group.rg.name
  webapp_storage_account_name = "${var.resource_name_prefix}${var.webapp_storage_account_name}"
  kv_id                       = module.network.kv_id
  tags                        = local.common_tags
}

# Create web application resources
module "webapp" {
  source = "./modules/azure-web"

  environment          = var.environment
  location             = var.azure_region
  resource_group       = azurerm_resource_group.rg.name
  resource_name_prefix = var.resource_name_prefix
  asp_sku              = var.asp_sku
  webapp_worker_count  = var.webapp_worker_count
  webapp_name          = var.webapp_name
  webapp_app_settings  = local.webapp_app_settings
  tags                 = local.common_tags
  depends_on           = [module.network]
}

module "postgres" {
  source = "./modules/azure-postgresql"

  environment          = var.environment
  location             = var.azure_region
  resource_group       = azurerm_resource_group.rg.name
  resource_name_prefix = var.resource_name_prefix
  vnet_id              = module.network.vnet_id
  vnet_name            = module.network.vnet_name
  kv_id                = module.network.kv_id
  days_to_expire       = var.days_to_expire
}

module "frontdoor" {
  source = "./modules/azure-frontdoor"

  resource_name_prefix = var.resource_name_prefix
  tags                 = local.common_tags
  resource_group       = azurerm_resource_group.rg.name
  default_hostname     = module.webapp.default_hostname
  depends_on           = [module.webapp]
}

module "acr" {
  source = "./modules/azure-container-registry"

  resource_name_prefix = var.resource_name_prefix
  resource_group       = azurerm_resource_group.rg.name
  location             = var.azure_region
  acr_sku              = var.acr_sku
  tags                 = local.common_tags
  admin_enabled        = var.admin_enabled
}
