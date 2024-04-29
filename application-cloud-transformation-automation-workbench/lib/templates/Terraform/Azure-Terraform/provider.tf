terraform {
  required_version = ">= 0.13"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "2.40.0"
    }
    azuredevops = {
      source  = "microsoft/azuredevops"
      version = "0.1.0"
    }
  }
}

provider "azurerm" {
  features {}
  # Configuration options
}

provider "azuredevops" {
  # Configuration options
}