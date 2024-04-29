/*
output "login_server" {
  description = "Container Registry URL"
  value       = azurerm_container_registry.container_registry.login_server
}

output "admin_username" {
  description = "Container Registry UserName"
  value       = azurerm_container_registry.container_registry.admin_username
}

output "admin_password" {
  description = "Container Registry Password"
  value       = azurerm_container_registry.container_registry.admin_password
}

*/

output "login_server" {
  description = "Container Registry URL"
  value       = data.azurerm_container_registry.container_registry.login_server
}

output "admin_username" {
  description = "Container Registry UserName"
  value       = data.azurerm_container_registry.container_registry.admin_username
}

output "admin_password" {
  description = "Container Registry Password"
  value       = data.azurerm_container_registry.container_registry.admin_password
}