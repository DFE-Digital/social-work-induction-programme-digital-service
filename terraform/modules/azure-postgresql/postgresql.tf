resource "azurerm_subnet" "postgres_sn" {
  name                 = "${var.resource_name_prefix}-sn"
  resource_group_name  = var.resource_group
  virtual_network_name = var.vnet_name
  address_prefixes     = ["10.0.2.0/24"]
  service_endpoints    = ["Microsoft.Storage"]
  delegation {
    name = "fs"
    service_delegation {
      name = "Microsoft.DBforPostgreSQL/flexibleServers"
      actions = [
        "Microsoft.Network/virtualNetworks/subnets/join/action",
      ]
    }
  }
}

resource "azurerm_private_dns_zone" "private_dns" {
  name                = "pvtswipdb.postgres.database.azure.com"
  resource_group_name = var.resource_group

  tags = var.tags

  lifecycle {
    ignore_changes = [
      tags["Environment"],
      tags["Product"],
      tags["Service Offering"]
    ]
  }
}

resource "azurerm_private_dns_zone_virtual_network_link" "vnetlink" {
  name                  = "swipdb-priv-vnet-link"
  private_dns_zone_name = azurerm_private_dns_zone.private_dns.name
  virtual_network_id    = var.vnet_id
  resource_group_name   = var.resource_group
  depends_on            = [azurerm_subnet.postgres_sn]
  tags                  = var.tags

  lifecycle {
    ignore_changes = [
      tags["Environment"],
      tags["Product"],
      tags["Service Offering"]
    ]
  }
}

resource "random_password" "password" {
  length = 16
}

resource "azurerm_postgresql_flexible_server" "swipdb" {
  name                          = "${var.resource_name_prefix}swipdb"
  resource_group_name           = var.resource_group
  location                      = var.location
  version                       = "15"
  delegated_subnet_id           = azurerm_subnet.postgres_sn.id
  private_dns_zone_id           = azurerm_private_dns_zone.private_dns.id
  administrator_login           = "psqladmin"
  administrator_password        = random_password.password.result
  zone                          = "3"
  storage_mb                    = 32768
  sku_name                      = "B_Standard_B1ms"
  backup_retention_days         = 7
  geo_redundant_backup_enabled  = false
  public_network_access_enabled = false
  depends_on                    = [azurerm_private_dns_zone_virtual_network_link.vnetlink]

  lifecycle {
    ignore_changes = [
      tags["Environment"],
      tags["Product"],
      tags["Service Offering"],
      administrator_password
    ]
  }

  #checkov:skip=CKV_AZURE_136:Geo-redundant backups not required
}

locals {
  // we're using timeadd and we can't pass the day directly need to be hours
  days_to_hours = var.days_to_expire * 24
  // expiration date need to be in a specific format as well
  expiration_date = timeadd(formatdate("YYYY-MM-DD'T'HH:mm:ssZ", timestamp()), "${local.days_to_hours}h")
}

resource "azurerm_key_vault_secret" "database_password" {
  name            = "postgresql--admin--password"
  value           = azurerm_postgresql_flexible_server.swipdb.administrator_password
  key_vault_id    = var.kv_id
  content_type    = "password"
  expiration_date = local.expiration_date

  lifecycle {
    ignore_changes = [value, expiration_date]
  }
}
