
module "amaze_awssecretmanager_ecr" {  
  source   = "../02-ECR"
  repository_name     = "awssecretmanager-service"
  tags = {
    Project ="Amaze"
    Environment = "Dev"
  }
}

module "amaze__awssecretmanager_ecsfargate" {  
  source   = "../07-ECSFargate"
  cluster_name = "amaze-ecs"
  vpc_id =   "vpc-59fa2a32"
  subnets = ["subnet-1ad9ef60","subnet-8621bfca","subnet-bf34ccd4"]
  source_cidr_blocks =  ["172.31.16.0/20", "172.31.32.0/20", "172.31.0.0/20"]
  name     = "awssecretmanager-service-svc"
  aws_region = "us-east-2"
  cpu = "512"
  memory = "1024"
  scale_min_capacity = 1
  scale_max_capacity = 2

  container_name = "awssecretmanager-service"  
  container_image = module.amaze_awssecretmanager_ecr.repository_url
  container_log_retention_in_days = "3"
  container_port = "8080"
  container_toport = "8080"
  container_definitions = "../07-ECSFargate/templates/ecs/myapp.json.tpl"
  desired_count = 1
  assign_public_ip = true
  environment = []
  # [               
  #               { "name" : "AWS_ACCESS_KEY_ID", "value" : "" },
  #               { "name" : "AWS_SECRET_KEY", "value" : "" },
  #               { "name" : "AWS_REGION", "value" : "us-east-2" }
  #               ]
  secrets = [ ]

  lb_arn =  "arn:aws:elasticloadbalancing:us-east-2:628030530634:loadbalancer/app/amaze-alb/0a6384257728df50"
  aws_lb_listener_rule_priority = "91"

  target_group_name = "awssecretmanager-service-tg"  
  target_group_port = "80"  
  health_check_path = "/awssecretmanager/liveness"
  health_check_grace_period_seconds = 120
  path_pattern = "/awssecretmanager*"


  tags = {
    Project ="Amaze"
    Environment = "Dev"
  }
}



output "awssecretmanager-service_ecs_service_cluster" {
    value       = "amaze-ecs"
    description = "The Amazon Resource Name (ARN) of cluster which the service runs on."
  }
  
  output "awssecretmanager-service_ecs_lb_url" {
    value       = "amaze-alb-1961089221.us-east-2.elb.amazonaws.com"
    description = "The Amazon Resource Name (ARN) of cluster which the service runs on."
  }
  
  output "awssecretmanager-service_ecr_url" {
    value       =  module.amaze_awssecretmanager_ecr.repository_url
    description = "aws repository_url"
  }
  