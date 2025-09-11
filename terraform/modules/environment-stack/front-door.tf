resource "azurerm_cdn_frontdoor_profile" "front_door_profile_web" {
  name                = "${var.resource_name_prefix}-fd-profile-web"
  resource_group_name = azurerm_resource_group.rg_primary.name
  sku_name            = var.frontdoor_sku
  tags                = var.tags

  lifecycle {
    ignore_changes = [tags]
  }
}
