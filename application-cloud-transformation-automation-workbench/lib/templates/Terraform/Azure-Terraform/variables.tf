# App Infrastructure
variable "name" {}

# Resource Group
variable "location" {}
variable "environment" {}

# Container Registry
# variable "admin_enabled" {}
# variable "sku" {}
# variable "georeplication_locations" { }

variable "container-registry-name" {}
variable "container-registry-resource_group_name" {}

# PostgreSQL 
variable "administrator_login" {}
variable "administrator_login_password" {}

# App Service
variable "sku-tier" {}
variable "sku-size" {}
variable "site_config-linux_fx_version" {}
variable "env_variable" {}

# Redis Cache
variable "capacity" {}
variable "family" {}
variable "sku_name" {}
variable "enable_non_ssl_port" {}
variable "minimum_tls_version" {}