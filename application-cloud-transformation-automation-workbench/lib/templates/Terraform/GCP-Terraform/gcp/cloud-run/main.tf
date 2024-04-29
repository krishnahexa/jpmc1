resource "google_cloud_run_service" "cloud-run" {
  name     = var.name
  location = var.location
  project = var.project

  template {
    spec {
      containers {
        image = var.image
      }
    }
  }

  traffic {
    percent         = 100
    latest_revision = true
  }
}
