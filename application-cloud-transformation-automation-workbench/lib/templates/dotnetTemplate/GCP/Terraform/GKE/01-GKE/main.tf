module "gke" {
  source                     = "terraform-google-modules/kubernetes-engine/google"
  project_id                 = "gke-tf-393105"
  name                       = "gke-test-1"
  region                     = "us-central1"
  zones                      = ["us-central1-a", "us-central1-b", "us-central1-f"]
  network                    = "default"
  subnetwork                 = "default"
  ip_range_pods              = ""
  ip_range_services          = ""
  http_load_balancing        = false
  network_policy             = false
  horizontal_pod_autoscaling = true
  filestore_csi_driver       = false

  node_pools = [
    {
      name                      = "default-node-pool"
      machine_type              = "e2-medium"
      node_locations            = "us-central1-b,us-central1-c,us-central1-a"
      min_count                 = 1
      max_count                 = 9
      local_ssd_count           = 0
      spot                      = false
      disk_size_gb              = 10
      disk_type                 = "pd-standard"
      image_type                = "COS_CONTAINERD"
      enable_gcfs               = false
      enable_gvnic              = false
      auto_repair               = true
      auto_upgrade              = true
      service_account           = "gke-tf-393105@gke-tf-393105.iam.gserviceaccount.com"
      preemptible               = false
      initial_node_count        = 1
      horizontal_pod_autoscaling = true
    },
  ]

  node_pools_oauth_scopes = {
    all = [
      "https://www.googleapis.com/auth/logging.write",
      "https://www.googleapis.com/auth/monitoring",
    ]
  }

  node_pools_labels = {
    all = {}

    default-node-pool = {
      default-node-pool = true
    }
  }

  node_pools_metadata = {
    all = {}

    default-node-pool = {
      node-pool-metadata-custom-value = "my-node-pool"
    }
  }

  node_pools_taints = {
    all = []

    default-node-pool = [
      {
        key    = "default-node-pool"
        value  = true
        effect = "PREFER_NO_SCHEDULE"
      },
    ]
  }

  node_pools_tags = {
    all = []

    default-node-pool = [
      "default-node-pool",
    ]
  }
}

resource "kubernetes_deployment" "gke_deploy_1" {

  metadata {

    name = "gke-deploy-1"

    labels = {

      test = "$$appName$$"

    }

  }




  spec {

    replicas = 3




    selector {

      match_labels = {

        test = "$$appName$$"

      }

    }




    template {

      metadata {

        labels = {

          test = "$$appName$$"

        }

      }




      spec {

        container {

          image = "gcr.io/gke-tf-393105/gke-container@sha256:a9b2cdc22fa8f085c1dbe37a51580ab5509ca1a967b1c8886193752ceb275a52"

          name  = "gke-container"




          resources {

            limits = {

              cpu    = "0.5"

              memory = "512Mi"

            }

            requests = {

              cpu    = "250m"

              memory = "50Mi"

            }

          }




          liveness_probe {

            http_get {

              path = "/"

              port = 8080




              http_header {

                name  = "X-Custom-Header"

                value = "Awesome"

              }

            }




            initial_delay_seconds = 3

            period_seconds        = 3

          }

        }

      }

    }

  }

}


