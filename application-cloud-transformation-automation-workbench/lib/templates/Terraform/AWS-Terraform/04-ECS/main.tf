
resource "aws_ecs_cluster" "main" {    
  name = var.name

    tags = merge(
    var.tags,
    var.tags,
    {
      Name = var.name != null ? var.name : var.name
    },
  )
  
}
