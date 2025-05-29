resource "azurerm_monitor_action_group" "stack_action_group" {
  name                = "${var.resource_name_prefix}-ag-support"
  resource_group_name = azurerm_resource_group.rg_primary.name
  short_name          = "${var.resource_name_prefix}-ags"

  email_receiver {
    name          = "support"
    email_address = var.email_support_address
  }
  tags = var.tags
}
