resource "azurerm_resource_group" "resource_group" {
  name     = "${var.name}Rg"
  location = var.location
  tags = {
    environment = var.environment
  }
}