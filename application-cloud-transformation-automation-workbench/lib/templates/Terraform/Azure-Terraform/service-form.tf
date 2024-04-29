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