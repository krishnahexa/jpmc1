resource "google_app_engine_application" "app" {       //creates an app engine
  project     = "terraform-gcp-389609"           //Provide the ProjectId
  location_id = "us-central"
}

resource "google_project_service" "service" {
  project = "terraform-gcp-389609"                           //Provide the ProjectId
  service = "appengineflex.googleapis.com"
  disable_dependent_services = false
}

resource "google_service_account" "custom_service_account" { 
  project      = google_project_service.service.project                   //creates custom service account            
  account_id   = "my-account"
  display_name = "Custom Service Account"
}

resource "google_project_iam_member" "gae_api" {              
  project = google_project_service.service.project        //assigning role to custom service account
  role    = "roles/compute.networkUser"
  member  = "serviceAccount:${google_service_account.custom_service_account.email}"
}

resource "google_project_iam_member" "logs_writer" {
  project = google_project_service.service.project        //assigning role to custom service account
  role    = "roles/logging.logWriter"
  member  = "serviceAccount:${google_service_account.custom_service_account.email}"
}

resource "google_project_iam_member" "storage_viewer" {
  project = google_project_service.service.project       //assigning role to custom service account
  role    = "roles/storage.objectViewer"
  member  = "serviceAccount:${google_service_account.custom_service_account.email}"
}
