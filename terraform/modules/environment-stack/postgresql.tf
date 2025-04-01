resource "azurerm_private_dns_zone" "private_dns_postgres" {
  name                = "${var.resource_name_prefix}-pvtswipdb.postgres.database.azure.com"
  resource_group_name = azurerm_resource_group.rg.name

  tags = var.tags

  lifecycle {
    ignore_changes = [tags]
  }
}

resource "azurerm_private_dns_zone_virtual_network_link" "private_dns_vnet_link" {
  name                  = "${var.resource_name_prefix}-priv-dns-vnet-link"
  private_dns_zone_name = azurerm_private_dns_zone.private_dns_postgres.name
  virtual_network_id    = azurerm_virtual_network.vnet_stack.id
  resource_group_name   = azurerm_resource_group.rg.name
  tags                  = var.tags

  lifecycle {
    ignore_changes = [tags]
  }
}

resource "azurerm_subnet" "sn_postgres" {
  name                 = "${var.resource_name_prefix}-sn"
  resource_group_name  = azurerm_resource_group.rg.name
  virtual_network_name = azurerm_virtual_network.vnet_stack.name
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

  lifecycle {
    ignore_changes = [delegation]
  }
}

resource "random_password" "password" {
  length = 16
}

resource "azurerm_postgresql_flexible_server" "swipdb" {
  name                          = "${var.resource_name_prefix}-swipdb"
  resource_group_name           = azurerm_resource_group.rg.name
  location                      = var.location
  version                       = "15"
  delegated_subnet_id           = azurerm_subnet.sn_postgres.id
  private_dns_zone_id           = azurerm_private_dns_zone.private_dns_postgres.id
  administrator_login           = "psqladmin"
  administrator_password        = random_password.password.result
  zone                          = "3"
  storage_mb                    = 32768
  sku_name                      = "B_Standard_B1ms"
  backup_retention_days         = 7
  geo_redundant_backup_enabled  = false
  public_network_access_enabled = false
  tags                          = var.tags

  lifecycle {
    ignore_changes = [tags]
  }

  #checkov:skip=CKV_AZURE_136:Geo-redundant backups not required
  #checkov:skip=CKV2_AZURE_57:Private link not required as using nsg
}

resource "azurerm_private_endpoint" "pe_postgres" {
  name                = "${var.resource_name_prefix}-pe-postgres"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg.name
  subnet_id           = azurerm_subnet.sn_postgres.id
  custom_network_interface_name = "${var.resource_name_prefix}-cnin-postgres"
  tags = var.tags

  ip_configuration {
    name                       = "${var.resource_name_prefix}-ipc-postgres"
    private_ip_address         = "10.0.2.2"
    subresource_name           = "postgresqlServer" 
  }

 private_service_connection {
    name                           = "${var.resource_name_prefix}-psc-postgres"
    private_connection_resource_id = azurerm_postgresql_flexible_server.swipdb.id
    is_manual_connection           = false
    subresource_names              = [ "postgresqlServer" ]
  }

  private_dns_zone_group {
    name                 = azurerm_private_dns_zone.private_dns_postgres.name
    private_dns_zone_ids = [ azurerm_private_dns_zone.private_dns_postgres.id ]
  }
}

resource "azurerm_key_vault_secret" "database_password" {
  name            = "postgresql--admin--password"
  value           = azurerm_postgresql_flexible_server.swipdb.administrator_password
  key_vault_id    = azurerm_key_vault.kv.id
  content_type    = "password"

  // TODO: Managed expiry of db passwords
  //expiration_date = local.expiration_date

  lifecycle {
    ignore_changes = [value, expiration_date]
  }
}
