
# Create a VPC
module "amaze_vpc" {
  # source = "github.com/"
  source   = "../01-Network"
  name = "amaze-vpcs"
  cidr = "10.0.0.0/16"
  public_subnets     = ["10.0.1.0/24", "10.0.2.0/24", "10.0.3.0/24", "10.0.4.0/24"]
  private_subnets = ["10.0.11.0/24", "10.0.12.0/24", "10.0.13.0/24", "10.0.14.0/24"]
  database_subnets = ["10.0.21.0/24", "10.0.22.0/24", "10.0.23.0/24", "10.0.24.0/24"]
  elasticache_subnets = ["10.0.31.0/24", "10.0.32.0/24", "10.0.33.0/24", "10.0.34.0/24"]
  azs =   ["us-west-2a","us-west-2b","us-west-2c","us-west-2d"]
  enable_nat_gateway = true
  single_nat_gateway = true
  one_nat_gateway_per_az = false 
  create_database_nat_gateway_route = true 
  create_database_internet_gateway_route = false 
  create_elasticache_subnet_route_table = false
  create_database_subnet_route_table = false
  tags = {
    Project ="Amaze"
    Environment = "Dev"
  }
}


module "amaze_alb" {  
  source   = "../03-ALB"
  name     = "amaze-alb"
  subnets =   module.amaze_vpc.public_subnets
  vpc_id =   module.amaze_vpc.vpc_id
  tags =  {
    Project ="Amaze"
    Environment = "Dev"
  }
}


module "amaze_ecs" {  
  source   = "../04-ECS"
  name     = "amaze-ecs"  
  tags =  {
    Project ="Amaze"
    Environment = "Dev"
  }
}


module "amaze_ec" {  
  source   = "../05-ElasticCache"
  namespace     = "amaze"
  name = "amaze-cache"
  vpc_id =  module.amaze_vpc.vpc_id
  stage = "dev"
  subnets = module.amaze_vpc.elasticache_subnets
  elasticache_subnet_group_name = module.amaze_vpc.elasticache_subnet_group_name
  instance_type = "cache.t2.micro"
  family = "redis5.0"
  parameter_group_name = "default.redis5.0.cluster.on"
  engine_version = "5.0.6"
  at_rest_encryption_enabled = true
  transit_encryption_enabled = true
  apply_immediately = true
  automatic_failover_enabled = true 
  availability_zones  =  ["us-west-2a","us-west-2b","us-west-2c","us-west-2d"]  
  allowed_cidr_blocks = ["10.0.11.0/24", "10.0.12.0/24", "10.0.13.0/24", "10.0.14.0/24"]
  auth_token = "amaze123456Light$#"  
  replication_group_id = "amaze-cache-rg"
  cluster_mode_enabled = true
  cluster_mode_replicas_per_node_group = 1
  cluster_mode_num_node_groups = 1
  #cluster_size = 1
  tags = {
    Project ="Amaze"
    Environment = "Dev"
  }
}

module "amaze_db" {  
  source   = "../06-AuroraPostgres"
  name     = "amaze-aurora"
  replica_count = 1
  vpc_id =  module.amaze_vpc.vpc_id
  db_subnet_group_name =  module.amaze_vpc.database_subnet_group_name
  allowed_cidr_blocks = ["10.0.11.0/24", "10.0.12.0/24", "10.0.13.0/24", "10.0.14.0/24"]
  instance_type = "db.r5.large"
  publicly_accessible = false
  database_name = "amazehi"
  username = "root"
  password = "Homeinsurance#$%"
  deletion_protection = false 
  backup_retention_period = 7
  preferred_backup_window =  "02:00-03:00"
  preferred_maintenance_window = "sun:05:00-sun:06:00"
  port = "5432"
  apply_immediately = true 
  auto_minor_version_upgrade  = true
  db_parameter_group_name = "default.aurora-postgresql11"
  db_cluster_parameter_group_name = "default.aurora-postgresql11"
  storage_encrypted = true
  engine = "aurora-postgresql"
  engine_version = "11.6"  
  ca_cert_identifier = "rds-ca-2019"
  tags = {
    Project ="Amaze"
    Environment = "Dev"
  }
}



output "ecs_service_cluster" {
  value       = module.amaze_ecs.name
  description = "The Amazon Resource Name (ARN) of cluster which the service runs on."
}

output "ecs_lb_arn" {
  value       = module.amaze_alb.this_lb_arn
  description = "The Amazon Resource Name (ARN) of cluster which the service runs on."
}

output "ecs_lb_url" {
  value       = module.amaze_alb.this_lb_dns_name
  description = "The DNS URL of cluster which the service runs on."
}

output "vpc_id" {
  value       = module.amaze_vpc.vpc_id
  description = "aws vpc id"
}

output "vpc_id_private_subnets" {
  description = "List of IDs of private subnets"
  value       =  module.amaze_vpc.private_subnets
}

output "private_subnets_cidr_blocks" {
  description = "List of cidr_blocks of private subnets"
  value       = module.amaze_vpc.private_subnets_cidr_blocks
}

output "redis_endpoint" {
  value       =  module.amaze_ec.endpoint
  description = "aws redis_endpoint"
}

output "db_endpoint" {
  value       =  module.amaze_db.this_rds_cluster_endpoint
  description = "aws this_rds_cluster_endpoint"
}
output "db_reader_endpoint" {
  value       =  module.amaze_db.this_rds_cluster_reader_endpoint
  description = "aws this_rds_cluster_reader_endpoint"
}



