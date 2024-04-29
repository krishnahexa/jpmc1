### Copy Required Module from Amaze Terraform 
source   = "../07-ECSFargate"
source   = "../02-ECR"

## Run 

$ export AWS_ACCESS_KEY_ID="anaccesskey"
$ export AWS_SECRET_ACCESS_KEY="asecretkey"
$ export AWS_DEFAULT_REGION="us-west-2"

$ terraform init

$ terraform plan

$ terraform apply



Reference

https://www.terraform.io/docs/providers/aws/r/ecr_repository.html

https://github.com/azavea/terraform-aws-ecr-repository

https://askubuntu.com/questions/983351/how-to-install-terraform-in-ubuntu