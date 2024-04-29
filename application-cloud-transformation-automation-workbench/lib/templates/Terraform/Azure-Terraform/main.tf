module "resource_group" {
  source      = "./azure/resource-group"
  name        = var.name
  location    = var.location
  environment = var.environment
}

/*
module "container-registry" {
  source                   = "./azure/container-registry"
  name                     = var.name
  location                 = var.location
  environment              = var.environment
  resource_group_name      = module.resource_group.resource_group_name
  admin_enabled            = var.admin_enabled
  sku                      = var.sku
  georeplication_locations = var.georeplication_locations
}
*/

module "container-registry" {
  source                                 = "./azure/container-registry"
  container-registry-name                = var.container-registry-name
  container-registry-resource_group_name = var.container-registry-resource_group_name
}

module "postgresql_server" {
  source                       = "./azure/postgresql"
  name                         = var.name
  location                     = var.location
  resource_group_name          = module.resource_group.resource_group_name
  administrator_login          = var.administrator_login
  administrator_login_password = var.administrator_login_password
}

module "app_service" {
  source                       = "./azure/app-service"
  name                         = var.name
  location                     = var.location
  environment                  = var.environment
  env_variable                 = var.env_variable
  resource_group_name          = module.resource_group.resource_group_name
  sku-tier                     = var.sku-tier
  sku-size                     = var.sku-size
  site_config-linux_fx_version = var.site_config-linux_fx_version
  app_settings-reg_url         = module.container-registry.login_server
  app_settings-reg_username    = module.container-registry.admin_username
  app_settings-reg_password    = module.container-registry.admin_password
}

module "redis" {
  source              = "./azure/redis"
  name                = var.name
  location            = var.location
  resource_group_name = module.resource_group.resource_group_name
  capacity            = var.capacity
  family              = var.family
  sku_name            = var.sku_name
  enable_non_ssl_port = var.enable_non_ssl_port
  minimum_tls_version = var.minimum_tls_version
}