output "resource_group_id" {
  description = "Resource Group Id"
  value       = azurerm_resource_group.resource_group.id
}

output "resource_group_name" {
  description = "Resource Group Name"
  value       = azurerm_resource_group.resource_group.name
}