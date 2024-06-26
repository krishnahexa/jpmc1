#
# Security Group Resources
#
resource "aws_security_group" "main" {
  count  = var.enabled && var.use_existing_security_groups == false ? 1 : 0
  vpc_id = var.vpc_id
  name   = "${var.name}-cache-sg"
  tags   = var.tags
}

resource "aws_security_group_rule" "egress" {
  count             = var.enabled && var.use_existing_security_groups == false ? 1 : 0
  description       = "Allow all egress traffic"
  from_port         = var.port
  to_port           = var.toport
  protocol          = "tcp"
  cidr_blocks       = ["0.0.0.0/0"]  
  security_group_id = join("", aws_security_group.main.*.id)
  type              = "egress"
}

resource "aws_security_group_rule" "ingress_security_groups_tcp" {
  count                    = var.enabled && var.use_existing_security_groups == false ? length(var.allowed_security_groups) : 0
  description              = "Allow inbound traffic from existing Security Groups ${var.port}"
  from_port                = var.port
  to_port                  = var.port
  protocol                 = "tcp"
  source_security_group_id = var.allowed_security_groups[count.index]
  security_group_id        = join("", aws_security_group.main.*.id)
  type                     = "ingress"
}
resource "aws_security_group_rule" "ingress_security_groups_ssl" {
  count                    = var.enabled && var.use_existing_security_groups == false ? length(var.allowed_security_groups) : 0
  description              = "Allow inbound traffic from existing Security Groups ${var.toport}"
  from_port                = var.toport
  to_port                  = var.toport
  protocol                 = "tcp"
  source_security_group_id = var.allowed_security_groups[count.index]
  security_group_id        = join("", aws_security_group.main.*.id)
  type                     = "ingress"
}

resource "aws_security_group_rule" "ingress_cidr_blocks" {
  count             = var.enabled && var.use_existing_security_groups == false && length(var.allowed_cidr_blocks) > 0 ? 1 : 0
  description       = "Allow inbound traffic from CIDR blocks"
  from_port         = var.port
  to_port           = var.toport
  protocol          = "tcp"
  cidr_blocks       = var.allowed_cidr_blocks
  security_group_id = join("", aws_security_group.main.*.id)
  type              = "ingress"
}
locals {
  elasticache_subnet_group_name = var.elasticache_subnet_group_name
}
# locals {
#   elasticache_subnet_group_name = var.elasticache_subnet_group_name != "" ? var.elasticache_subnet_group_name : join("", aws_elasticache_subnet_group.main.*.name)
# }

# resource "aws_elasticache_subnet_group" "main" {
#   count      = var.enabled && var.elasticache_subnet_group_name == "" && length(var.subnets) > 0 ? 1 : 0
#   name       = "${var.name}-Cache-sg"
#   subnet_ids = var.subnets
# }

# resource "aws_elasticache_parameter_group" "main" {
#   count  = var.enabled ? 1 : 0
#   name   = "${var.name}-Cache-pg"
#   family = var.family

#   dynamic "parameter" {
#     for_each = var.cluster_mode_enabled ? concat([{ "name" = "cluster-enabled", "value" = "yes" }], var.parameter) : var.parameter
#     content {
#       name  = parameter.value.name
#       value = parameter.value.value
#     }
#   }
# }

resource "aws_elasticache_replication_group" "main" {
  count = var.enabled ? 1 : 0

  auth_token                    = var.transit_encryption_enabled ? var.auth_token : null
  replication_group_id          = var.replication_group_id == "" ?"${var.name}-cache-gi" : var.replication_group_id
  replication_group_description = "${var.name}-Cache-rg"
  node_type                     = var.instance_type
  number_cache_clusters         = var.cluster_mode_enabled ? null : var.cluster_size
  port                          = var.port
  parameter_group_name          = var.parameter_group_name
  # parameter_group_name          = join("", aws_elasticache_parameter_group.main.*.name)
  availability_zones            = var.cluster_mode_enabled ? null : slice(var.availability_zones, 0, var.cluster_size)
  automatic_failover_enabled    = var.automatic_failover_enabled
  subnet_group_name             = local.elasticache_subnet_group_name
  security_group_ids            = var.use_existing_security_groups ? var.existing_security_groups : [join("", aws_security_group.main.*.id)]
  maintenance_window            = var.maintenance_window
  notification_topic_arn        = var.notification_topic_arn
  engine_version                = var.engine_version
  at_rest_encryption_enabled    = var.at_rest_encryption_enabled
  transit_encryption_enabled    = var.transit_encryption_enabled
  snapshot_window               = var.snapshot_window
  snapshot_retention_limit      = var.snapshot_retention_limit
  apply_immediately             = var.apply_immediately

  tags = var.tags

  dynamic "cluster_mode" {
    for_each = var.cluster_mode_enabled ? ["true"] : []
    content {
      replicas_per_node_group = var.cluster_mode_replicas_per_node_group
      num_node_groups         = var.cluster_mode_num_node_groups
    }
  }

}

#
# CloudWatch Resources
#
resource "aws_cloudwatch_metric_alarm" "cache_cpu" {
  count               = var.enabled ? 1 : 0
  alarm_name          = "${var.name}-cpu-utilization"
  alarm_description   = "Redis cluster CPU utilization"
  comparison_operator = "GreaterThanThreshold"
  evaluation_periods  = "1"
  metric_name         = "CPUUtilization"
  namespace           = "AWS/ElastiCache"
  period              = "300"
  statistic           = "Average"

  threshold = var.alarm_cpu_threshold_percent

  dimensions = {
    CacheClusterId ="${var.name}-CacheClusterId"
  }

  alarm_actions = var.alarm_actions
  ok_actions    = var.ok_actions
  depends_on    = [aws_elasticache_replication_group.main]
}

resource "aws_cloudwatch_metric_alarm" "cache_memory" {
  count               = var.enabled ? 1 : 0
  alarm_name          = "${var.name}-freeable-memory"
  alarm_description   = "Redis cluster freeable memory"
  comparison_operator = "LessThanThreshold"
  evaluation_periods  = "1"
  metric_name         = "FreeableMemory"
  namespace           = "AWS/ElastiCache"
  period              = "60"
  statistic           = "Average"

  threshold = var.alarm_memory_threshold_bytes

  dimensions = {
    CacheClusterId = "${var.name}-CacheClusterId"
  }

  alarm_actions = var.alarm_actions
  ok_actions    = var.ok_actions
  depends_on    = [aws_elasticache_replication_group.main]
}