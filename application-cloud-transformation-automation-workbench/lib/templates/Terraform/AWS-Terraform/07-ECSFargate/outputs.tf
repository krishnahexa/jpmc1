output "ecs_service_id" {
  value       = join("", aws_ecs_service.main.*.id)
  description = "The Amazon Resource Name (ARN) that identifies the service."
}

output "ecs_service_name" {
  value       = join("", aws_ecs_service.main.*.name)
  description = "The name of the service."
}

output "ecs_service_cluster" {
  value       = join("", aws_ecs_service.main.*.cluster)
  description = "The Amazon Resource Name (ARN) of cluster which the service runs on."
}

output "ecs_service_iam_role" {
  value       = join("", aws_ecs_service.main.*.iam_role)
  description = "The ARN of IAM role used for ELB."
}

output "ecs_service_desired_count" {
  value       = join("", aws_ecs_service.main.*.desired_count)
  description = "The number of instances of the task definition."
}

output "security_group_id" {
  value       = join("", aws_security_group.main.*.id)
  description = "The ID of the ECS Service security group."
}

output "security_group_arn" {
  value       = join("", aws_security_group.main.*.arn)
  description = "The ARN of the ECS Service security group."
}

output "security_group_vpc_id" {
  value       = join("", aws_security_group.main.*.vpc_id)
  description = "The VPC ID of the ECS Service security group."
}

output "security_group_owner_id" {
  value       = join("", aws_security_group.main.*.owner_id)
  description = "The owner ID of the ECS Service security group."
}

output "security_group_name" {
  value       = join("", aws_security_group.main.*.name)
  description = "The name of the ECS Service security group."
}


output "target_group_name" {
  value       = join("", aws_alb_target_group.app.*.name)
  description = "The name of the ECS Service security group."
}

output "security_group_description" {
  value       = join("", aws_security_group.main.*.description)
  description = "The description of the ECS Service security group."
}

output "security_group_ingress" {
  value       = flatten(aws_security_group.main.*.ingress)
  description = "The ingress rules of the ECS Service security group."
}

output "security_group_egress" {
  value       = flatten(aws_security_group.main.*.egress)
  description = "The egress rules of the ECS Service security group."
}

output "ecs_task_definition_arn" {
  value       = join("", aws_ecs_task_definition.main.*.arn)
  description = "Full ARN of the Task Definition (including both family and revision)."
}

output "ecs_task_definition_family" {
  value       = join("", aws_ecs_task_definition.main.*.family)
  description = "The family of the Task Definition."
}

output "ecs_task_definition_revision" {
  value       = join("", aws_ecs_task_definition.main.*.revision)
  description = "The revision of the task in a particular family."
}

output "iam_role_arn" {
  value       = join("", aws_iam_role.main.*.arn)
  description = "The Amazon Resource Name (ARN) specifying the IAM Role."
}

output "iam_role_create_date" {
  value       = join("", aws_iam_role.main.*.create_date)
  description = "The creation date of the IAM Role."
}

output "iam_role_unique_id" {
  value       = join("", aws_iam_role.main.*.unique_id)
  description = "The stable and unique string identifying the IAM Role."
}

output "iam_role_name" {
  value       = join("", aws_iam_role.main.*.name)
  description = "The name of the IAM Role."
}

output "iam_role_description" {
  value       = join("", aws_iam_role.main.*.description)
  description = "The description of the IAM Role."
}

output "iam_policy_id" {
  value       = join("", aws_iam_policy.main.*.id)
  description = "The IAM Policy's ID."
}

output "iam_policy_arn" {
  value       = join("", aws_iam_policy.main.*.arn)
  description = "The ARN assigned by AWS to this IAM Policy."
}

output "iam_policy_description" {
  value       = join("", aws_iam_policy.main.*.description)
  description = "The description of the IAM Policy."
}

output "iam_policy_name" {
  value       = join("", aws_iam_policy.main.*.name)
  description = "The name of the IAM Policy."
}

output "iam_policy_path" {
  value       = join("", aws_iam_policy.main.*.path)
  description = "The path of the IAM Policy."
}

output "iam_policy_document" {
  value       = join("", aws_iam_policy.main.*.policy)
  description = "The policy document of the IAM Policy."
}
