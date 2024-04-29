data "aws_lb" "selected" {
  arn  = "${var.lb_arn}"    
}

data "aws_lb_listener" "selected80" {
  load_balancer_arn = "${data.aws_lb.selected.arn}"
  port              = 80
}
data "aws_vpc" "selected" {
  id = "${var.vpc_id}"
}

data "aws_ecs_cluster" "selected" {
  cluster_name  = var.cluster_name
}



resource "aws_alb_target_group" "app" {
  name        = var.target_group_name
  port        = var.target_group_port
  protocol    = "HTTP"
  vpc_id      = data.aws_vpc.selected.id
  target_type = "ip"  

  health_check {
    healthy_threshold   = var.healthy_threshold
    interval            = var.healthy_interval
    protocol            = "HTTP"
    matcher             = "200"
    timeout             = var.healthy_timeout
    path                = var.healthy_check_path
    unhealthy_threshold = var.unhealthy_threshold
  }
}


# Redirect all traffic from the ALB to the target group
# resource "aws_alb_listener" "front_end" {
#   load_balancer_arn = var.lb_arn
#   port              = var.alb_listener_port
#   protocol          = "HTTP"

#   default_action {
#     target_group_arn = aws_alb_target_group.app.arn
#     type             = "forward"

#   }
# }

# Set up CloudWatch group and log stream and retain logs for 30 days
resource "aws_cloudwatch_log_group" "myapp_log_group" {
  name              = "/ecs/${var.cloudwatch_name}"
  retention_in_days = var.container_log_retention_in_days
  
}

resource "aws_cloudwatch_log_stream" "myapp_log_stream" {
  name           = "${var.cloudwatch_name}-log-stream"
  log_group_name = aws_cloudwatch_log_group.myapp_log_group.name
}

# aws_lb_listener_rule
resource "aws_lb_listener_rule" "main" {
  listener_arn = data.aws_lb_listener.selected80.arn
  priority     = var.aws_lb_listener_rule_priority

  action {
    type             = "forward"
    target_group_arn = aws_alb_target_group.app.arn
  }

  condition {
    path_pattern {
      values = [var.path_pattern]
    }
  }  
}

# ecs service
resource "aws_ecs_service" "main" {
  #count = var.enabled ? 1 : 0
  name = var.name
  task_definition = aws_ecs_task_definition.main[0].arn
  cluster = data.aws_ecs_cluster.selected.arn
  desired_count = var.desired_count
  deployment_maximum_percent = var.deployment_maximum_percent
  deployment_minimum_healthy_percent = var.deployment_minimum_healthy_percent
  deployment_controller {
    type = var.deployment_controller_type
  }
  network_configuration {
    subnets         = var.subnets
    security_groups = [aws_security_group.main[0].id]  
    assign_public_ip = var.assign_public_ip
  } 
  load_balancer {
    target_group_arn = aws_alb_target_group.app.arn    
    container_name = var.container_name   
    container_port = var.container_port
  }
  health_check_grace_period_seconds = var.health_check_grace_period_seconds
  platform_version = var.platform_version
  launch_type = "FARGATE"
  scheduling_strategy = "REPLICA"
  lifecycle {
    ignore_changes = [desired_count]
  }
}
resource "aws_security_group" "main" {
  count = var.enabled ? 1 : 0
  name   = local.security_group_name
  vpc_id = var.vpc_id
  tags   = merge({ "Name" = local.security_group_name }, var.tags)
}

locals {
  security_group_name = "${var.name}-ecs-fargate-sg"
}

resource "aws_security_group_rule" "ingress" {
  count = var.enabled ? 1 : 0
  type              = "ingress"
  from_port         = var.container_port
  to_port           = var.container_toport
  protocol          = "tcp"
  cidr_blocks       = var.source_cidr_blocks
  security_group_id = aws_security_group.main[0].id
}

resource "aws_security_group_rule" "egress" {
  count = var.enabled ? 1 : 0
  type              = "egress"
  from_port         = 0
  to_port           = 0
  protocol          = "-1"
  cidr_blocks       = ["0.0.0.0/0"]
  security_group_id = aws_security_group.main[0].id
}


data "template_file" "myapp" {
  template = file( var.container_definitions )

  vars = {
    app_image      = var.container_image
    app_port       = var.container_port
    fargate_cpu    = var.cpu
    fargate_memory = var.memory
    aws_region     = var.aws_region
    app_name       = var.container_name      
    app_name_cw       = var.cloudwatch_name 
    environment     = jsonencode (var.environment)
    secrets     = jsonencode (var.secrets)
  }
}


resource "aws_ecs_task_definition" "main" {
  count = var.enabled ? 1 : 0
  family =  "${var.name}-task-def"
  execution_role_arn = var.create_ecs_task_execution_role ? join("", aws_iam_role.main.*.arn) : var.ecs_task_execution_role_arn  
  container_definitions = data.template_file.myapp.rendered 
  cpu = var.cpu
  memory = var.memory
  requires_compatibilities = var.requires_compatibilities
  network_mode = "awsvpc"  
  tags = merge({ "Name" = var.name }, var.tags)
}
resource "aws_iam_role" "main" {
  count = local.enabled_ecs_task_execution

  name               = local.iam_name
  assume_role_policy = data.aws_iam_policy_document.assume_role_policy.json
  path               = var.iam_path
  description        = var.description
  tags               = merge({ "Name" = local.iam_name }, var.tags)
}
data "aws_iam_policy_document" "assume_role_policy" {
  statement {
    actions = ["sts:AssumeRole"]

    principals {
      type        = "Service"
      identifiers = ["ecs-tasks.amazonaws.com"]
    }
  }   
}
resource "aws_iam_policy" "main" {
  count = local.enabled_ecs_task_execution
  name        = local.iam_name
  policy      = data.aws_iam_policy.ecs_task_execution.policy
  path        = var.iam_path
  description = var.description
}
resource "aws_iam_role_policy_attachment" "main" {
  count = local.enabled_ecs_task_execution
  role       = aws_iam_role.main[0].name
  policy_arn = aws_iam_policy.main[0].arn
}
locals {
  iam_name                   = "${var.name}-ecs-task-execution"
  enabled_ecs_task_execution = var.enabled ? 1 : 0 && var.create_ecs_task_execution_role ? 1 : 0
}
data "aws_iam_policy" "ecs_task_execution" {
  arn = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy"
}


# auto_scaling.tf

resource "aws_appautoscaling_target" "target" {
  service_namespace  = "ecs"
  resource_id        = "service/${var.cluster_name}/${aws_ecs_service.main.name}"
  scalable_dimension = "ecs:service:DesiredCount"
  min_capacity       = var.scale_min_capacity
  max_capacity       = var.scale_max_capacity
}

# Automatically scale capacity up by one
resource "aws_appautoscaling_policy" "up" {
  name               = "${var.name}_scale_up"
  service_namespace  = "ecs"
  resource_id        = "service/${var.cluster_name}/${aws_ecs_service.main.name}"
  scalable_dimension = "ecs:service:DesiredCount"

  step_scaling_policy_configuration {
    adjustment_type         = "ChangeInCapacity"
    cooldown                = 60
    metric_aggregation_type = "Maximum"

    step_adjustment {
      metric_interval_lower_bound = 0
      scaling_adjustment          = 1
    }
  }

  depends_on = [aws_appautoscaling_target.target]
}

# Automatically scale capacity down by one
resource "aws_appautoscaling_policy" "down" {
  name               = "${var.name}_scale_down"
  service_namespace  = "ecs"
  resource_id        = "service/${var.cluster_name}/${aws_ecs_service.main.name}"
  scalable_dimension = "ecs:service:DesiredCount"

  step_scaling_policy_configuration {
    adjustment_type         = "ChangeInCapacity"
    cooldown                = 60
    metric_aggregation_type = "Maximum"

    step_adjustment {
      metric_interval_upper_bound = 0
      scaling_adjustment          = -1
    }
  }

  depends_on = [aws_appautoscaling_target.target]
}

# CloudWatch alarm that triggers the autoscaling up policy
resource "aws_cloudwatch_metric_alarm" "service_cpu_high" {
  alarm_name          = "${var.name}_cpu_utilization_high"
  comparison_operator = "GreaterThanOrEqualToThreshold"
  evaluation_periods  = "2"
  metric_name         = "CPUUtilization"
  namespace           = "AWS/ECS"
  period              = "60"
  statistic           = "Average"
  threshold           = "85"

  dimensions = {
    ClusterName = var.cluster_name
    ServiceName = aws_ecs_service.main.name
  }

  alarm_actions = [aws_appautoscaling_policy.up.arn]
}

# CloudWatch alarm that triggers the autoscaling down policy
resource "aws_cloudwatch_metric_alarm" "service_cpu_low" {
  alarm_name          = "${var.name}_cpu_utilization_low"
  comparison_operator = "LessThanOrEqualToThreshold"
  evaluation_periods  = "2"
  metric_name         = "CPUUtilization"
  namespace           = "AWS/ECS"
  period              = "60"
  statistic           = "Average"
  threshold           = "10"

  dimensions = {
    ClusterName = var.cluster_name
    ServiceName = aws_ecs_service.main.name
  }

  alarm_actions = [aws_appautoscaling_policy.down.arn]
}
