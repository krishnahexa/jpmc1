resource "azuredevops_project" "project" {
  project_name       = var.project_name
  description        = var.project_description
  visibility         = "private"
  version_control    = "Git"
  work_item_template = "Agile"
}


resource "azuredevops_variable_group" "variable_group" {
  project_id   = azuredevops_project.project.id
  name         = "Test Variable Group"
  description  = "Test Variable Group Description"
  allow_access = true

  variable {
    name  = "key"
    value = "value"
  }

  variable {
    name      = "Account Password"
    value     = "p@ssword123"
    is_secret = true
  }
}