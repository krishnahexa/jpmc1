

module "amaze_ecr" {  
  source   = "../02-ECR"
  repository_name     = "servicename"
  tags = {
    Project ="Amaze"
    Environment = "Dev"
  }
}



module "amaze_ecsfargate_servicename" {  
  source   = "../07-ECSFargate"
  cluster_name = "update ecs.name"
  vpc_id =   "update vpc_id"
  subnets = ["subnet-xxxxx","subnet-xxxxx","subnet-xxxx"]
  source_cidr_blocks =  ["10.0.1.0/24", "10.0.2.0/24", "10.0.3.0/24", "10.0.4.0/24"]
  name     = "servicename-svc"
  aws_region = "update us-west-2"
  cpu = "512"
  memory = "1024"
  scale_min_capacity = 1
  scale_max_capacity = 6

  container_name = "servicename"  
  cloudwatch_name = "servicename_cw"
  container_image = module.amaze_ecr.repository_url
  container_log_retention_in_days = "3"
  container_port = "8080"
  container_toport = "8080"
  container_definitions = "../07-ECSFargate/templates/ecs/myapp.json.tpl"
  desired_count = 0
  assign_public_ip = false
  environment = [
                { "name" : "SPRING_DATASOURCE_URL", "value" : "update db server" },
                { "name" : "SPRING_DATASOURCE_PASSWORD", "value" : "update db password" },
                { "name" : "SPRING_REDIS_HOST", "value" : "update redis db server" },
                { "name" : "SPRING_REDIS_PASSWORD", "value" : "update redis db password" }
                ]

  lb_arn =  "update lb_arn"
  aws_lb_listener_rule_priority = "80"

  target_group_name = "servicename-tg"  
  target_group_port = "80"  
  healthy_check_path = "/servicename/actuator"
  health_check_grace_period_seconds = 120
  path_pattern = "/servicename*"


  tags = {
    Project ="Amaze"
    Environment = "Dev"
  }
}


# Step to Update below item from Common Terraform output
# Update the ECS Cluster Name
# Update VPC ID
# Update Private Subnet Name
# Update Public ALB Subnet cidr blocks for open security group port
# Update the ALB ARN Name

