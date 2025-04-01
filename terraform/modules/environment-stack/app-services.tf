resource "azurerm_subnet" "sn_webapps" {
  name                              = "${var.resource_name_prefix}-subnet-webapps"
  resource_group_name               = azurerm_resource_group.rg.name
  virtual_network_name              = azurerm_virtual_network.vnet_stack.name
  address_prefixes                  = ["10.0.3.0/24"]
  private_endpoint_network_policies = "Disabled"
  service_endpoints                 = ["Microsoft.Sql"]

  delegation {
    name = "delegation"

    service_delegation {
      name    = "Microsoft.Web/serverFarms"
      actions = ["Microsoft.Network/virtualNetworks/subnets/join/action", "Microsoft.Network/virtualNetworks/subnets/prepareNetworkPolicies/action"]
    }
  }

  lifecycle {
    ignore_changes = [delegation]
  }
}

resource "azurerm_service_plan" "asp" {
  name                = "${var.resource_name_prefix}-asp"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg.name
  os_type             = "Linux"
  sku_name            = var.asp_sku
  tags                = var.tags

  lifecycle {
    ignore_changes = [tags]
  }
}

resource "azurerm_monitor_autoscale_setting" "asp_autoscale" {
  name                = "${var.resource_name_prefix}-asp-autoscale-rule"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg.name
  target_resource_id  = azurerm_service_plan.asp.id
  tags                = var.tags

  profile {
    name = "defaultProfile"

    capacity {
      default = var.autoscale_rule_default_capacity
      minimum = var.autoscale_rule_minimum_capacity
      maximum = var.autoscale_rule_maximum_capacity
    }

    rule {
      metric_trigger {
        metric_name        = "CpuPercentage"
        metric_resource_id = azurerm_service_plan.asp.id
        time_grain         = "PT1M"
        statistic          = "Average"
        time_window        = "PT5M"
        time_aggregation   = "Average"
        operator           = "GreaterThan"
        threshold          = 70
      }

      scale_action {
        direction = "Increase"
        type      = "ChangeCount"
        value     = "1"
        cooldown  = "PT1M"
      }
    }

    rule {
      metric_trigger {
       metric_name        =  "CpuPercentage"
        metric_resource_id = azurerm_service_plan.asp.id
        time_grain         = "PT1M"
        statistic          = "Average"
        time_window        = "PT5M"
        time_aggregation   = "Average"
        operator           = "LessThan"
        threshold          = 20
      }

      scale_action {
        direction = "Decrease"
        type      = "ChangeCount"
        value     = "1"
        cooldown  = "PT1M"
      }
    }
  }
  
  lifecycle {
    ignore_changes = [tags]
  }
}

resource "azurerm_postgresql_firewall_rule" "app_subnet_rule" {
  name                = "AllowAppSubnet"
  resource_group_name = azurerm_resource_group.rg.name
  server_name         = azurerm_postgresql_flexible_server.swipdb.name
  start_ip_address    = "10.0.3.0"
  end_ip_address      = "10.0.3.255"
}
