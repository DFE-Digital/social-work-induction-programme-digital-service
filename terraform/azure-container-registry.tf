// TODO: This will work while we create environments within the same dev subscription, but
// not when we start using separate test and production subscriptions.

data "azurerm_container_registry" "acr_data" {
  count               = var.create_and_own_container_registry ? 0 : 1
  name                = var.acr_name
  resource_group_name = var.acr_resource_group
}

resource "azurerm_container_registry" "acr" {
  count               = var.create_and_own_container_registry ? 1 : 0
  name                = var.acr_name
  resource_group_name = module.stack.resource_group_name
  location            = var.azure_region
  sku                 = var.acr_sku
  admin_enabled       = var.admin_enabled
  tags                = local.common_tags

  lifecycle {
    ignore_changes = [tags]
  }

  #checkov:skip=CKV_AZURE_164:Content Trusted images not available on Basic SKU
  #checkov:skip=CKV_AZURE_237:Dedicate data endpoint not available on Basic SKU
  #checkov:skip=CKV_AZURE_163:Vulnerability scanned before image creation
  #checkov:skip=CKV_AZURE_233:Zone redundancy not needed and not available on Basic SKU
  #checkov:skip=CKV_AZURE_165:Geo-replication not needed and not available on Basic SKU
  #checkov:skip=CKV_AZURE_166:Image quarantine not available on Basic SKU
  #checkov:skip=CKV_AZURE_167:Retention policy not available on Basic SKU
  #checkov:skip=CKV_AZURE_139:Disable public network access not available on Basic SKU
}

locals {
  acr_id = var.create_and_own_container_registry ? azurerm_container_registry.acr[0].id : data.azurerm_container_registry.acr_data[0].id
}
