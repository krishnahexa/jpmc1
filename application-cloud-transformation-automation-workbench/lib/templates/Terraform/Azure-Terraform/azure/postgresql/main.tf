
resource "azurerm_postgresql_server" "postgresql_server" {
  name                = "${lower(var.name)}-postgresql"
  location            = var.location
  resource_group_name = var.resource_group_name
  administrator_login          = var.administrator_login
  administrator_login_password = var.administrator_login_password

  sku_name   = "GP_Gen5_4"
  version    = "9.6"
  storage_mb = 20480

  backup_retention_days        = 7
  geo_redundant_backup_enabled = false
  auto_grow_enabled            = true

  public_network_access_enabled    = true
  ssl_enforcement_enabled          = true
  ssl_minimal_tls_version_enforced = "TLS1_2"
}