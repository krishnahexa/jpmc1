# Set the project ID and region
$projectId = "terraform-gcp-389609"                 
$region = "us-central1"
# Set the path to the application source code
$sourcePath = "C:/Users/AmazeWorkbench/Downloads/EmployeeService"
# Set the project ID for the gcloud command
& gcloud config set project $projectId
# Deploy the App Engine application
& gcloud app deploy --project=$projectId --version=v1 --quiet $sourcePath    //sourcepath of app.yaml
