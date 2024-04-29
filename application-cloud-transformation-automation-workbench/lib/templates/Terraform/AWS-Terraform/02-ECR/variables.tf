variable "repository_name" {
  default     = "amaze_repo"
  type        = string
  description = " Name of the repository"
}
variable "repository_scan" {
  default     = false
  type        = bool
  description = "Configuration block that defines image scanning configuration for the repository"
}

variable "attach_lifecycle_policy" {
  default     = false
  type        = bool
  description = "If true, an ECR lifecycle policy will be attached"
}

variable "lifecycle_policy" {
  default     = ""
  type        = string
  description = "Expire untagged images older than 7 days"
}
variable "tags" {
  description = "A map of tags to add to all target groups"
  type        = map(string)
  default     = {
    Project ="Amaze"
    Environment = "Dev"
  }
}