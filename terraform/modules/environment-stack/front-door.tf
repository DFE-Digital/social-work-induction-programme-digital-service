resource "azurerm_cdn_frontdoor_profile" "front_door_profile_web" {
  name                = "${var.resource_name_prefix}-fd-profile-web"
  resource_group_name = azurerm_resource_group.rg_primary.name
  sku_name            = var.frontdoor_sku
  tags                = var.tags

  lifecycle {
    ignore_changes = [tags]
  }
}

# Magic Links Module - Only created when enabled
module "magic-links" {
  count  = var.magic_links_enabled ? 1 : 0
  source = "./magic-links"

  resource_name_prefix = var.resource_name_prefix
  magic_links_enabled  = var.magic_links_enabled
  moodle_instances     = var.moodle_instances
  frontdoor_sku        = var.frontdoor_sku
  tags                 = var.tags

  key_vault_id           = azurerm_key_vault.kv.id
  frontdoor_profile_id   = azurerm_cdn_frontdoor_profile.front_door_profile_web.id
  frontdoor_profile_name = azurerm_cdn_frontdoor_profile.front_door_profile_web.name
  resource_group_name    = azurerm_resource_group.rg_primary.name
}
