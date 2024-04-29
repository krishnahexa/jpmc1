output "id" {
  value       = join("", aws_elasticache_replication_group.main.*.id)
  description = "Redis cluster ID"
}

output "security_group_id" {
  value       = join("", aws_security_group.main.*.id)
  description = "Security group ID"
}

output "port" {
  value       = var.port
  description = "Redis port"
}

output "endpoint" {
  value       = var.cluster_mode_enabled ? join("", aws_elasticache_replication_group.main.*.configuration_endpoint_address) : join("", aws_elasticache_replication_group.main.*.primary_endpoint_address)
  description = "Redis primary endpoint"
}