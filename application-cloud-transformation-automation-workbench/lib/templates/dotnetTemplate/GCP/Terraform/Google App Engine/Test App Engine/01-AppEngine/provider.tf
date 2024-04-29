terraform {
  required_providers {                         
    google = {
      source  = "hashicorp/google"
      version = "4.65.2"
    }
  }
}
provider "google" {
  project     = "terraform-gcp-389609"                      //Provide the ProjectId
  region      = "us-central1"
  zone = "us-central1-b"
}