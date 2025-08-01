resource "azurerm_private_dns_zone" "private_dns_postgres" {
  name                = "${var.resource_name_prefix}-pvtswipdb.postgres.database.azure.com"
  resource_group_name = azurerm_resource_group.rg_primary.name

  tags = var.tags

  lifecycle {
    ignore_changes = [tags]
  }
}

resource "azurerm_private_dns_zone_virtual_network_link" "private_dns_vnet_link" {
  name                  = "${var.resource_name_prefix}-priv-dns-vnet-link"
  private_dns_zone_name = azurerm_private_dns_zone.private_dns_postgres.name
  virtual_network_id    = azurerm_virtual_network.vnet_stack.id
  resource_group_name   = azurerm_resource_group.rg_primary.name
  tags                  = var.tags

  lifecycle {
    ignore_changes = [tags]
  }
}

resource "azurerm_subnet" "sn_postgres" {
  name                 = "${var.resource_name_prefix}-sn-postgres"
  resource_group_name  = azurerm_resource_group.rg_primary.name
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
  length = 25
  special          = true
  override_special = "!@#^*_-"
}

resource "azurerm_postgresql_flexible_server" "swipdb" {
  name                          = "${var.resource_name_prefix}-swipdb"
  resource_group_name           = azurerm_resource_group.rg_primary.name
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

# When the Postgres sql server is assigned an A record in the private DNS zone,
# the record has a unique suffix such as f87687686 in front of it. This means we can't connect
# to the server as we can't determine the DNS name in advance. This is a workaround to retrieve
# the server's IP address so we can then assign a usable IP address below.
data "external" "postgres_private_ip" {
  program = [
    "bash", "-c", <<EOT
      ip=$(az network private-dns record-set list \
        --resource-group ${azurerm_resource_group.rg_primary.name} \
        --zone-name ${azurerm_private_dns_zone.private_dns_postgres.name} \
        --query "[?type=='Microsoft.Network/privateDnsZones/A'].aRecords[0].ipv4Address | [0]" \
        --output tsv)
      jq -n --arg ip "$ip" '{"ip_address": $ip}'
    EOT
  ]
  depends_on = [
    azurerm_private_dns_zone.private_dns_postgres,
    azurerm_postgresql_flexible_server.swipdb
  ]
}

# Assign a DNS A record which is actually useful!
resource "azurerm_private_dns_a_record" "dns_a_postgres" {
  name                = azurerm_postgresql_flexible_server.swipdb.name
  zone_name           = azurerm_private_dns_zone.private_dns_postgres.name
  resource_group_name = azurerm_resource_group.rg_primary.name
  ttl                 = "3600"
  records             = [data.external.postgres_private_ip.result.ip_address]
  tags                = var.tags

  lifecycle {
    ignore_changes = [tags]
  }
}

resource "azurerm_key_vault_secret" "database_password" {
  name            = "Database-AdminPassword"
  value           = azurerm_postgresql_flexible_server.swipdb.administrator_password
  key_vault_id    = azurerm_key_vault.kv.id
  content_type    = "password"

  // TODO: Managed expiry of db passwords
  //expiration_date = local.expiration_date

  lifecycle {
    ignore_changes = [value, expiration_date]
  }

  depends_on = [ azurerm_key_vault_access_policy.kv_gh_ap ]

  #checkov:skip=CKV_AZURE_41:Ensure that the expiration date is set on all secrets
}
