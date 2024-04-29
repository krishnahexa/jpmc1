# GCP Infrastructure
variable "region" {}
variable "project" {}

variable "database_name" {}
# variable "database_version" {}
variable "database_region" {}
variable "database_tier" {}

variable "service_name" {}
variable "service_image" {}

# variable "services" {
#   type = list(any)
#   default = [
#     {
#       name  = "hello-service"
#       image = "us-docker.pkg.dev/cloudrun/container/hello"
#     }
#   ]
# }