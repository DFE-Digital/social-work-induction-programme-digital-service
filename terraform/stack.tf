module "stack" {
  source = "./modules/environment-stack"

  environment                 = var.environment
  location                    = var.azure_region
  resource_name_prefix        = var.resource_name_prefix
  asp_sku                     = var.asp_sku
  acr_sku                     = var.acr_sku
  admin_enabled               = var.admin_enabled
  webapp_storage_account_name = var.webapp_storage_account_name
  tags                        = local.common_tags
}


