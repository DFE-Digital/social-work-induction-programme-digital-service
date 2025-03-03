resource "azurerm_container_registry" "acr" {
  name                = "${var.resource_name_prefix}acr"
  resource_group_name = var.resource_group
  location            = var.location
  sku                 = var.acr_sku
  admin_enabled       = var.admin_enabled
  #public_network_access_enabled = var.public_network_access_enabled
  tags = var.tags

  #checkov:skip=CKV_AZURE_164:Content Trusted images not available on Basic SKU
  #checkov:skip=CKV_AZURE_237:Dedicate data endpoint not available on Basic SKU
  #checkov:skip=CKV_AZURE_163:Vulnerability scanned before image creation
  #checkov:skip=CKV_AZURE_233:Zone redundancy not needed and not available on Basic SKU
  #checkov:skip=CKV_AZURE_165:Geo-replication not needed and not available on Basic SKU
  #checkov:skip=CKV_AZURE_166:Image quarantine not available on Basic SKU
  #checkov:skip=CKV_AZURE_167:Retention policy not available on Basic SKU
}
