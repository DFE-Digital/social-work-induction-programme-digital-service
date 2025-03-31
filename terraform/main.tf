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
  depends_on                  = [module.network]
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
  tags                 = local.common_tags
  moodle_db_name       = var.moodle_db_name
  depends_on           = [module.network]
}

module "acr" {
  source = "./modules/azure-container-registry"

  resource_name_prefix = var.resource_name_prefix
  resource_group       = azurerm_resource_group.rg.name
  location             = var.azure_region
  acr_sku              = var.acr_sku
  tags                 = local.common_tags
  admin_enabled        = var.admin_enabled
  depends_on           = [module.network]
}

# Create web application resources
module "webapp" {
  source = "./modules/azure-web"

  environment           = var.environment
  location              = var.azure_region
  resource_group        = azurerm_resource_group.rg.name
  resource_name_prefix  = var.resource_name_prefix
  asp_sku               = var.asp_sku
  webapp_worker_count   = var.webapp_worker_count
  webapp_name           = var.webapp_name
  tags                  = local.common_tags
  kv_id                 = module.network.kv_id
  moodle_db_name        = var.moodle_db_name
  postgres_username     = module.postgres.postgres_username
  postgres_secret_uri   = module.postgres.postgres_secret_uri
  moodle_db_type        = var.moodle_db_type
  moodle_db_host        = module.postgres.postgres_db_host
  moodle_db_prefix      = var.moodle_db_prefix
  moodle_admin_user     = var.moodle_admin_user
  moodle_admin_password = var.moodle_admin_password
  moodle_admin_email    = var.moodle_admin_email
  moodle_site_fullname  = var.moodle_site_fullname
  moodle_site_shortname = var.moodle_site_shortname
  moodle_web_port       = var.moodle_web_port
  acr_id                = module.acr.acr_id
  kv_vault_uri          = module.network.kv_vault_uri
  depends_on            = [module.network, module.postgres, module.acr]
}

module "frontdoor" {
  source = "./modules/azure-frontdoor"

  resource_name_prefix = var.resource_name_prefix
  tags                 = local.common_tags
  resource_group       = azurerm_resource_group.rg.name
  default_hostname     = module.webapp.default_hostname
  depends_on           = [module.webapp]
}
