
data "google_project" "project" {
  project_id  = "$$appName$$88906"
}

resource "google_service_account" "cloudbuild_service_account" {
  project    = "$$appName$$88906"
  # If using an existing service account, set the appropriate account_id and display_name
  account_id   = "$$appName$$88906"
  display_name = "$$appName$$"
}

resource "google_project_iam_member" "act_as" {
  project = data.google_project.project.project_id
  role    = "roles/iam.serviceAccountUser"
  member  = "serviceAccount:${google_service_account.cloudbuild_service_account.email}"
}

resource "google_project_iam_member" "logs_writer" {
  project = data.google_project.project.project_id
  role    = "roles/logging.logWriter"
  member  = "serviceAccount:${google_service_account.cloudbuild_service_account.email}"
}

resource "google_project_iam_member" "storage_admin" {
  project = data.google_project.project.project_id
  role    = "roles/storage.objectAdmin"
  member  = "serviceAccount:${google_service_account.cloudbuild_service_account.email}"
}

resource "google_cloudbuild_trigger" "service-account-trigger" {
  project  = "$$appName$$88906"
  trigger_template {
    branch_name = "main"
    repo_name   = "github_gcp-appengine-deploy-repo"
  }

  service_account = google_service_account.cloudbuild_service_account.id
  filename        = "cloudbuild.yaml"
  
  # Use the correct resource references for the dependencies
  depends_on = [
    google_project_iam_member.act_as,
    google_project_iam_member.logs_writer,
    google_project_iam_member.storage_admin
  ]
}
