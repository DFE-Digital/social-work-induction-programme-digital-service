resource "azurerm_subnet" "cae_subnet" {
  name                 = "${var.resource_name_prefix}-sn-container-apps"
  resource_group_name  = var.primary_resource_group
  virtual_network_name = azurerm_virtual_network.vnet_stack.name

  address_prefixes = ["10.0.8.0/24"]
}

resource "azurerm_container_app_environment" "env" {
  name                       = "${var.resource_name_prefix}-cae-env"
  location                   = var.location
  resource_group_name        = var.primary_resource_group
  log_analytics_workspace_id = azurerm_log_analytics_workspace.log_analytics_web.id
  infrastructure_subnet_id   = azurerm_subnet.cae_subnet.id

  tags = var.tags
}
