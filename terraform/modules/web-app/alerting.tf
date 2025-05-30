resource "azurerm_monitor_metric_alert" "frontdoor_back_end_health" {
  name                = "${var.resource_name_prefix}-mma-fd-back-end-health-${var.web_app_short_name}"
  resource_group_name = var.resource_group
  scopes              = [var.front_door_profile_web_id]
  description         = "Alert when Front Door backend health drops"
  frequency           = "PT1M"
  window_size         = "PT5M"
  severity            = 2
  enabled             = true

  criteria {
    metric_namespace = "Microsoft.Cdn/profiles"
    metric_name      = "OriginHealthPercentage"
    aggregation      = "Average"
    operator         = "LessThan"
    threshold        = 100
    dimension {
      name     = "OriginGroup"
      operator = "Include"
      values   = [azurerm_cdn_frontdoor_origin_group.front_door_origin_group_web.name]
    }
  }

  action {
    action_group_id = var.support_action_group_id
  }
  tags = var.tags
}
