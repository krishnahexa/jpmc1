/*
# Create New Container registry
resource "azurerm_container_registry" "container_registry" {
  name                     = "${var.name}ContainerRegistry"
  resource_group_name      = var.resource_group_name
  location                 = var.location
  sku                      = var.sku
  admin_enabled            = var.admin_enabled
  # georeplication_locations = var.georeplication_locations # Applicable only for premium sku.

  tags = {
      environment = var.environment
    }
}
*/

# Connect Existing Container Registry

data "azurerm_container_registry" "container_registry" {
  name                = var.container-registry-name
  resource_group_name = var.container-registry-resource_group_name
}


