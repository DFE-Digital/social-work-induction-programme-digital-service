resource "azurerm_log_analytics_workspace" "log_analytics_web" {
  name                = "${var.resource_name_prefix}-log-analytics"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg_primary.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
  daily_quota_gb      = 1

  lifecycle {
    ignore_changes = [tags]
  }
}

resource "azurerm_application_insights" "app_insights_web" {
  name                = "${var.resource_name_prefix}-app-insights"
  resource_group_name = azurerm_resource_group.rg_primary.name
  location            = var.location
  application_type    = "web"
  workspace_id        = azurerm_log_analytics_workspace.log_analytics_web.id
  tags                = var.tags

  lifecycle {
    ignore_changes = [tags]
  }
}
