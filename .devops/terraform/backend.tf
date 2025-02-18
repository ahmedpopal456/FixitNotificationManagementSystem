terraform {
  backend "azurerm" {
    storage_account_name = "__terraformstorageaccount__"
    container_name       = "tfstate"
    key                  = "fixit-nms.terraform.tfstate"
    access_key           = "__storagekey__"
  }

  required_version = "=1.1.3"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm",
      version = "=2.71.0"
    }
  }
}
