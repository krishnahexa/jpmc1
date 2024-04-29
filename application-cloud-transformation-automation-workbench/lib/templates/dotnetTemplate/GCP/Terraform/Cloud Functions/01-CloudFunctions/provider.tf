terraform {
  required_providers {
    google = {
      source  = "hashicorp/google"
      version = "4.65.2"
    }
  }
}

provider "google" {
  project     = "$$appName$$88906"
  region      = "us-central1"
  zone = "us-central1-b"
}