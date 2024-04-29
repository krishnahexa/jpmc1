
variable "name" {
  description = "name of the ecs cluster"
  type        = string
  default     = "Amaze-cluster"
}

variable "tags" {
  description = "A map of tags to add to ecs"
  type        = map(string)
  default     = {
    Project ="Amaze"
    Environment = "Dev"
  }
}
