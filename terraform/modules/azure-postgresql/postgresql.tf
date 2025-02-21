resource "azurerm_subnet" "postgres-sn" {
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
  name                = "${var.resource_name_prefix}swipdb.postgres.database.azure.com"
  resource_group_name = var.resource_group
}

resource "azurerm_private_dns_zone_virtual_network_link" "vnetlink" {
  name                  = "${var.resource_name_prefix}swipdb.com"
  private_dns_zone_name = azurerm_private_dns_zone.private_dns.name
  virtual_network_id    = var.vnet_id
  resource_group_name   = var.resource_group
  depends_on            = [azurerm_subnet.postgres_sn]
}

resource "azurerm_postgresql_flexible_server" "swipdb" {
  name                          = "${var.resource_name_prefix}swipdb"
  resource_group_name           = var.resource_group
  location                      = var.location
  version                       = "12"
  delegated_subnet_id           = azurerm_subnet.postgres_sn.id
  private_dns_zone_id           = azurerm_private_dns_zone.private_dns.id
  public_network_access_enabled = false
  administrator_login           = "psqladmin"
  administrator_password        = "H@Sh1CoR3!"
  zone                          = "1"

  storage_mb   = 32768
  storage_tier = "P4"

  sku_name   = "B_Standard_B1ms"
  depends_on = [azurerm_private_dns_zone_virtual_network_link.vnetlink]

}
