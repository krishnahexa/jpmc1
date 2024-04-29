resource "azurerm_app_service_plan" "app-service-plan" {
  name                = "${var.name}AppServicePlan"
  location            = var.location
  resource_group_name = var.resource_group_name
  kind                = "Linux"
  reserved            = true
  sku {
    tier = var.sku-tier
    size = var.sku-size
  }
  tags = {
    environment = var.environment
  }
}

resource "azurerm_app_service" "app-service" {
  name                = "${var.name}AppService"
  location            = var.location
  resource_group_name = var.resource_group_name
  app_service_plan_id = azurerm_app_service_plan.app-service-plan.id
  
  site_config {
    linux_fx_version = var.site_config-linux_fx_version
    always_on        = true
  }
  
  app_settings = merge({ 
    DOCKER_REGISTRY_SERVER_URL      = var.app_settings-reg_url
    DOCKER_REGISTRY_SERVER_USERNAME = var.app_settings-reg_username
    DOCKER_REGISTRY_SERVER_PASSWORD = var.app_settings-reg_password
  }, var.env_variable)

  tags = {
    environment = var.environment
  }
}