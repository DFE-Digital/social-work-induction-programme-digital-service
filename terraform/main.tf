provider "azurerm" {
  skip_provider_registration = "true"

  features {
    resource_group {
      prevent_deletion_if_contains_resources = false
    }

    key_vault {
      purge_soft_delete_on_destroy    = true
      recover_soft_deleted_key_vaults = true
    }
  }
}

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
