variable "name" {
  default = "helloworld-svc"
  type        = string
  description = "The name of ecs service."
}
variable "aws_region" {
  default = "us-west-2"
  type        = string
  description = "The name of aws_region"
}
variable "container_image" {
  default = "docker.io/sonamsamdupkhangsar/springboot-docker"
  type        = string
  description = " Docker image path"
}
variable "container_name" {
  default = "helloworld"
  type        = string
  description = "The name of the container to associate with the load balancer (as it appears in a container definition)."
}
variable "cloudwatch_name" {
  default = "helloworld_cw"
  type        = string
  description = "The name of the container to associate cloud watch."
}

variable "container_log_retention_in_days" {
  default = "1"
  type        = string
  description = " container_log_retention_in_days [1, 3, 5, 7, 14, 30, 60, 90, 120, 150, 180, 365, 400, 545, 731, 1827, 3653]"
}


variable "container_port" {
  default = "8080"
  type        = string
  description = "The port on the container to associate with the load balancer."
}
variable "container_toport" {
  default = "8080"
  type        = string
  description = "The to port on the container to associate with the load balancer."
}
variable "target_group_port" {
  default = "80"
  type        = string
  description = "The port on the container to associate with the load balancer."
}
variable "aws_lb_listener_rule_priority" {
  default = "80"
  type        = string
  description = "The port on the container to associate with the load balancer."
}
################
# Cluster
#################
variable "cluster_name" {
  default = "Amaze-cluster"
  type        = string
  description = "name of an ECS cluster."
}

variable "subnets" {
  default = ["subnet-0b98e3cd21bdffa72"]
  type        = list(string)
  description = "The subnets associated with the task or service."
}

# variable "target_group_arn" {
#   type        = string
#   description = "The ARN of the Load Balancer target group to associate with the service."
# }

 variable "target_group_name" {
   default =  "helloworld-tg"
   type        = string
   description = "The name of the Load Balancer target group to associate with the service."
 }
  variable "target_group_path" {
   default =  "/"
   type        = string
   description = "The name of the Load Balancer target group path (routing) to associate with the service."
 }

variable "vpc_id" {
  default = "vpc-0025f4bb5e57f6c76"
  type        = string
  description = "VPC Id to associate with ECS Service."
}

variable "lb_arn" {
  default    = "arn:aws:elasticloadbalancing:us-west-2:628030530634:loadbalancer/app/amaze-alb/affd0fc3c7a7e86f"
  type    = string
  description = "The arn of load balance "
}


variable "container_definitions" {
  default = "./templates/ecs/myapp.json.tpl"
  type        = string
  description = "A list of valid container definitions provided as a single valid JSON document."
}

variable "desired_count" {
  default     = 0
  type        = string
  description = "The number of instances of the task definition to place and keep running."
}

variable "deployment_maximum_percent" {
  default     = 200
  type        = string
  description = "The upper limit (as a percentage of the service's desiredCount) of the number of running tasks that can be running in a service during a deployment."
}

variable "deployment_minimum_healthy_percent" {
  default     = 100
  type        = string
  description = "The lower limit (as a percentage of the service's desiredCount) of the number of running tasks that must remain running and healthy in a service during a deployment."
}

variable "deployment_controller_type" {
  default     = "ECS"
  type        = string
  description = "Type of deployment controller. Valid values: CODE_DEPLOY, ECS."
}
#############
# true for public subnet
# false for private subnet ( nat gateway is attached)
#############
variable "assign_public_ip" {
  default     = true
  type        = string
  description = "Assign a public IP address to the ENI (Fargate launch type only). Valid values are true or false."
}

variable "health_check_grace_period_seconds" {
  default     = 60
  type        = string
  description = "Seconds to ignore failing load balancer health checks on newly instantiated tasks to prevent premature shutdown, up to 7200."
}

variable "healthy_check_path" {
  default     = "/"
  type        = string
  description = "path of health_check_path for conatiner"
}
variable "healthy_threshold" {
  default     = "5"
  type        = string
  description = " healthy_threshold for conatiner"
}
variable "healthy_interval" {
  default     = "60"
  type        = string
  description = " interval for conatiner"
}
variable "healthy_timeout" {
  default     = "5"
  type        = string
  description = " timeout for conatiner"
}
variable "unhealthy_threshold" {
  default     = "2"
  type        = string
  description = " unhealthy_threshold for conatiner"
}

variable "platform_version" {
  default     = "1.4.0"
  type        = string
  description = "The platform version on which to run your service."
}

variable "source_cidr_blocks" {
  default     = ["0.0.0.0/0"]
  type        = list(string)
  description = "List of source CIDR blocks."
}

variable "cpu" {
  default     = "512"
  type        = string
  description = "The number of cpu units used by the task."
}

variable "memory" {
  default     = "1024"
  type        = string
  description = "The amount (in MiB) of memory used by the task."
}

variable "requires_compatibilities" {
  default     = ["FARGATE"]
  type        = list(string)
  description = "A set of launch types required by the task. The valid values are EC2 and FARGATE."
}

variable "iam_path" {
  default     = "/"
  type        = string
  description = "Path in which to create the IAM Role and the IAM Policy."
}

variable "description" {
  default     = "Managed by Amaze Terraform"
  type        = string
  description = "The description of the all resources."
}

variable "tags" {
  default     = {
    Project ="Amaze"
    Environment = "Dev"
  }
  type        = map(string)
  description = "A mapping of tags to assign to all resources."
}


variable "enabled" {
  default     = true
  type        = string
  description = "Set to false to prevent the module from creating anything."
}

variable "create_ecs_task_execution_role" {
  default     = true
  type        = string
  description = "Specify true to indicate that ECS Task Execution IAM Role creation."
}

variable "ecs_task_execution_role_arn" {
  default     = ""
  type        = string
  description = "The ARN of the ECS Task Execution IAM Role."
}

variable "scale_min_capacity" {
  default     = "1"
  type        = string
  description = "scale_min_capacity"
}

variable "scale_max_capacity" {
  default     = "2"
  type        = string
  description = "scale_max_capacity"
}

variable "path_pattern" {
  default     = "/*"
  type        = string
  description = "path_pattern"
}

 variable "environment" {
   type = list(object({
     name  = string
     value = string
   }))
   description = "The environment variables to pass to the container. This is a list of maps"
   default     = []
 }
variable "secrets" {
   type = list(object({
     name  = string
     valueFrom = string
   }))
   description = "The environment variables to pass to the container. This is a list of maps"
   default     = []
 }
