resource "azurerm_key_vault_secret" "basic_auth_password" {
  name         = "Sites-BasicAuthPassword"
  value        = var.basic_auth_password
  key_vault_id = azurerm_key_vault.kv.id
  content_type = "password"

  // TODO: Managed expiry of db passwords
  //expiration_date = local.expiration_date

  lifecycle {
    ignore_changes = [expiration_date]
  }

  depends_on = [azurerm_key_vault_access_policy.kv_gh_ap]

  #checkov:skip=CKV_AZURE_41:Ensure that the expiration date is set on all secrets
}

################################################
# App Service Plan for Moodle application
################################################

resource "azurerm_subnet" "sn_moodle_app" {
  name                              = "${var.resource_name_prefix}-sn-moodle"
  resource_group_name               = azurerm_resource_group.rg_primary.name
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

resource "azurerm_service_plan" "asp_moodle_app" {
  name                = "${var.resource_name_prefix}-asp-moodle"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg_primary.name
  os_type             = "Linux"
  sku_name            = var.asp_sku_moodle
  tags                = var.tags

  lifecycle {
    ignore_changes = [tags]
  }

  #checkov:skip=CKV_AZURE_225:Ensure the App Service Plan is zone redundant
  #checkov:skip=CKV_AZURE_212:Ensure App Service has a minimum number of instances for failover
}

resource "azurerm_monitor_autoscale_setting" "asp_autoscale_moodle_app" {
  name                = "${var.resource_name_prefix}-asp-autoscale-moodle"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg_primary.name
  target_resource_id  = azurerm_service_plan.asp_moodle_app.id
  enabled             = var.moodle_minimum_instances != var.moodle_maximum_instances
  tags                = var.tags

  profile {
    name = "defaultProfile"

    capacity {
      default = var.moodle_default_instances
      minimum = var.moodle_minimum_instances
      maximum = var.moodle_maximum_instances
    }

    rule {
      metric_trigger {
        metric_name        = "CpuPercentage"
        metric_resource_id = azurerm_service_plan.asp_moodle_app.id
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
        metric_name        = "CpuPercentage"
        metric_resource_id = azurerm_service_plan.asp_moodle_app.id
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


resource "azurerm_postgresql_flexible_server_firewall_rule" "sqlfr_moodle_app" {
  name             = "AllowMoodleAppSubnet"
  server_id        = azurerm_postgresql_flexible_server.swipdb.id
  start_ip_address = "10.0.3.0"
  end_ip_address   = "10.0.3.255"
}

#############################################################################
# App Service Plan for Moodle maintenance - installation app / cron-jobs
#############################################################################

resource "azurerm_subnet" "sn_maintenance_apps" {
  name                              = "${var.resource_name_prefix}-sn-maintenance"
  resource_group_name               = azurerm_resource_group.rg_primary.name
  virtual_network_name              = azurerm_virtual_network.vnet_stack.name
  address_prefixes                  = ["10.0.4.0/24"]
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

resource "azurerm_service_plan" "asp_maintenance_apps" {
  name                = "${var.resource_name_prefix}-asp-maintenance"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg_primary.name
  os_type             = "Linux"
  sku_name            = var.asp_sku_maintenance
  tags                = var.tags

  lifecycle {
    ignore_changes = [tags]
  }

  #checkov:skip=CKV_AZURE_225:Ensure the App Service Plan is zone redundant
  #checkov:skip=CKV_AZURE_212:Ensure App Service has a minimum number of instances for failover
}

resource "azurerm_postgresql_flexible_server_firewall_rule" "sqlfr_maintenance_apps" {
  name             = "AllowMaintenanceAppsSubnet"
  server_id        = azurerm_postgresql_flexible_server.swipdb.id
  start_ip_address = "10.0.4.0"
  end_ip_address   = "10.0.4.255"
}

#############################################################################
# App Service Plan for service apps
#############################################################################

resource "azurerm_subnet" "sn_service_apps" {
  name                              = "${var.resource_name_prefix}-sn-services"
  resource_group_name               = azurerm_resource_group.rg_primary.name
  virtual_network_name              = azurerm_virtual_network.vnet_stack.name
  address_prefixes                  = ["10.0.5.0/24"]
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

resource "azurerm_service_plan" "asp_service_apps" {
  name                = "${var.resource_name_prefix}-asp-services"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg_primary.name
  os_type             = "Linux"
  sku_name            = var.asp_sku_services
  tags                = var.tags

  lifecycle {
    ignore_changes = [tags]
  }

  #checkov:skip=CKV_AZURE_225:Ensure the App Service Plan is zone redundant
  #checkov:skip=CKV_AZURE_212:Ensure App Service has a minimum number of instances for failover
}

resource "azurerm_monitor_autoscale_setting" "asp_autoscale_services" {
  name                = "${var.resource_name_prefix}-asp-autoscale-services"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg_primary.name
  target_resource_id  = azurerm_service_plan.asp_service_apps.id
  enabled             = var.service_apps_minimum_instances != var.service_apps_maximum_instances
  tags                = var.tags

  profile {
    name = "defaultProfile"

    capacity {
      default = var.service_apps_default_instances
      minimum = var.service_apps_minimum_instances
      maximum = var.service_apps_maximum_instances
    }

    rule {
      metric_trigger {
        metric_name        = "CpuPercentage"
        metric_resource_id = azurerm_service_plan.asp_service_apps.id
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
        metric_name        = "CpuPercentage"
        metric_resource_id = azurerm_service_plan.asp_service_apps.id
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

resource "azurerm_postgresql_flexible_server_firewall_rule" "sqlfr_services" {
  name             = "AllowServicesSubnet"
  server_id        = azurerm_postgresql_flexible_server.swipdb.id
  start_ip_address = "10.0.5.0"
  end_ip_address   = "10.0.5.255"
}

#############################################################################
# App Service Plan for notification service
#############################################################################

resource "azurerm_subnet" "sn_function_app" {
  name                              = "${var.resource_name_prefix}-sn-funcapp"
  resource_group_name               = azurerm_resource_group.rg_primary.name
  virtual_network_name              = azurerm_virtual_network.vnet_stack.name
  address_prefixes                  = ["10.0.6.0/24"]
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

resource "azurerm_service_plan" "asp_notification_service" {
  name                = "${var.resource_name_prefix}-asp-notification"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg_primary.name
  os_type             = "Linux"
  sku_name            = var.asp_sku_notification
  tags                = var.tags

  lifecycle {
    ignore_changes = [tags]
  }

  #checkov:skip=CKV_AZURE_225:Ensure the App Service Plan is zone redundant
  #checkov:skip=CKV_AZURE_212:Ensure App Service has a minimum number of instances for failover
}

resource "azurerm_service_plan" "asp_db_jobs" {
  name                = "${var.resource_name_prefix}-asp-db-jobs"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg_primary.name
  os_type             = "Linux"
  sku_name            = var.asp_sku_db_jobs
  tags                = var.tags

  lifecycle {
    ignore_changes = [tags]
  }

  #checkov:skip=CKV_AZURE_225:Ensure the App Service Plan is zone redundant
  #checkov:skip=CKV_AZURE_212:Ensure App Service has a minimum number of instances for failover
}
