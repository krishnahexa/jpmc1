resource "google_sql_database_instance" "postgres-11" {
  name             = var.name
  database_version = "POSTGRES_11"
  region           = var.region
  project         = var.project
  deletion_protection = false
  root_password = "postgres@123"

  settings {
    # Second-generation instance tiers are based on the machine
    # type. See argument reference below.
    tier = var.tier
  }
}
