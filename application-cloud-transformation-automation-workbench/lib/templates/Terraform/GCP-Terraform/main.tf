module "sql-database-instance" {
  source  = "./gcp/sql-database-instance"
  project = var.project
  name    = var.database_name
  # version = "POSTGRES_11" //var.database_version
  region = var.database_region
  tier   = var.database_tier
}

module "cloud-run" {
  source   = "./gcp/cloud-run"
  project  = var.project
  name     = var.service_name
  location = var.region
  image    = var.service_image
}

# module "cloud-run" {
#   dynamic "service" {
#     for_each = var.services
#     content {
#       source   = "./gcp/cloud-run"
#       name     = service.service_name
#       location = var.database_region
#       image    = service.service_image
#     }
#   }
# }
