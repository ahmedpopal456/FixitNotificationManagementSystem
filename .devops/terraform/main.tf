resource "azurerm_resource_group" "main" {
  name     = "${var.organization_name}-${var.environment_name}-${var.service_abbreviation}"
  location = var.location_name
}

resource "azurerm_storage_account" "main" {
  name                     = "${var.organization_name}${var.environment_name}${var.service_abbreviation}"
  resource_group_name      = azurerm_resource_group.main.name
  location                 = azurerm_resource_group.main.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_storage_account" "app" {
  for_each                 = toset(var.function_apps)
  name                     = "${var.organization_name}${var.environment_name}${var.service_abbreviation}${each.key}"
  resource_group_name      = azurerm_resource_group.main.name
  location                 = azurerm_resource_group.main.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_storage_queue" "main" {
  name                 = "${var.organization_name}-${var.environment_name}-${var.service_abbreviation}-queue"
  storage_account_name = azurerm_storage_account.main.name
}

resource "azurerm_notification_hub_namespace" "main" {
  name                = "${var.organization_name}${var.environment_name}Namespace"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  namespace_type      = "NotificationHub"
  sku_name            = "Free"
}

resource "azurerm_notification_hub" "main" {
  name                = "${var.organization_name}-${var.environment_name}-${var.service_abbreviation}-hub"
  namespace_name      = azurerm_notification_hub_namespace.main.name
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location

  gcm_credential {
    api_key = data.azurerm_key_vault_secret.gcm_key.value
  }

  apns_credential {
    application_mode  = var.apns_credential_application_mode
    bundle_id         = var.apns_credential_bundle_id
    key_id            = var.apns_credential_key_id
    team_id           = var.apns_credential_team_id
    token             = var.apns_credential_token
  }
}

resource "azurerm_notification_hub_authorization_rule" "main" {
  name                  = "RootManageSharedAccessKey"
  notification_hub_name = azurerm_notification_hub.main.name
  namespace_name        = azurerm_notification_hub_namespace.main.name
  resource_group_name   = azurerm_resource_group.main.name
  manage                = true
  send                  = true
  listen                = true
}

resource "azurerm_app_service_plan" "main" {
  name                = "${var.organization_name}-${var.environment_name}-${var.service_abbreviation}-service-plan"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  kind                = "FunctionApp"

  sku {
    tier = "Dynamic"
    size = "Y1"
  }
}

resource "azurerm_function_app" "main" {
  for_each                   = toset(var.function_apps)
  name                       = "${var.organization_name}-${var.environment_name}-${var.service_abbreviation}-${each.key}"
  location                   = azurerm_resource_group.main.location
  resource_group_name        = azurerm_resource_group.main.name
  app_service_plan_id        = azurerm_app_service_plan.main.id
  storage_account_name       = azurerm_storage_account.app[each.key].name
  storage_account_access_key = azurerm_storage_account.app[each.key].primary_access_key
  version                    = "~3"

  site_config {
    scm_type = "VSTSRM"
  }

  app_settings = {
    "AzureWebJobsStorage"             = azurerm_storage_account.app[each.key].primary_connection_string,
    "WEBSITE_ENABLE_SYNC_UPDATE_SITE" = "true",
    "WEBSITE_RUN_FROM_PACKAGE"        = "1",
    "APPINSIGHTS_INSTRUMENTATIONKEY"  = data.azurerm_application_insights.main.instrumentation_key,
    "WEBSITE_NODE_DEFAULT_VERSION"    = "10.14.1"
    "FUNCTIONS_WORKER_RUNTIME"        = "dotnet",

    "FIXIT-FMS-DB-CS" = "AccountEndpoint=${data.azurerm_cosmosdb_account.main.endpoint};AccountKey=${data.azurerm_cosmosdb_account.main.primary_key};",

    "FIXIT-NMS-DB-EP"            = data.azurerm_cosmosdb_account.main.endpoint,
    "FIXIT-NMS-DB-KEY"           = data.azurerm_cosmosdb_account.main.primary_key,
    "FIXIT-NMS-DB-INSTALLATIONS" = azurerm_cosmosdb_sql_container.main["installations"].name,
    "FIXIT-NMS-DB-NAME"          = var.organization_name,

    "FIXIT-NMS-QUEUE-NAME"        = azurerm_storage_queue.main.name,
    "FIXIT-NMS-STORAGEACCOUNT-CS" = azurerm_storage_account.main.primary_connection_string,
    "FIXIT-FMS-STORAGEACCOUNT-CS" = var.fms_connection_string,
    "FIXIT-NMS-ANH-CS"            = "Endpoint=sb://${azurerm_notification_hub_namespace.main.name}.servicebus.windows.net/;SharedAccessKeyName=${azurerm_notification_hub_authorization_rule.main.name};SharedAccessKey=${azurerm_notification_hub_authorization_rule.main.primary_access_key}",
    "FIXIT-NMS-ANH-NAME"          = azurerm_notification_hub.main.name

    "FIXIT-GOOGLE-DISTANCEMATRIX-URI" = "https://maps.googleapis.com/maps/api/distancematrix/json",
    "FIXIT-GOOGLE-API-KEY"            = data.azurerm_key_vault_secret.google_maps_api_key.value,
    "FIXIT-UMS-SERVICE-EP"            = "https://${var.organization_name}-${var.environment_name}-ums-api.azurewebsites.net/",
    "FIXIT-MDM-SERVICE-EP"            = "https://${var.organization_name}-${var.environment_name}-mdm-api.azurewebsites.net/",
  }
}

resource "azurerm_cosmosdb_sql_database" "main" {
  name                = var.organization_name
  resource_group_name = data.azurerm_cosmosdb_account.main.resource_group_name
  account_name        = data.azurerm_cosmosdb_account.main.name
  throughput          = 400
}

resource "azurerm_cosmosdb_sql_container" "main" {
  for_each            = var.cosmosdb_tables
  name                = each.value
  resource_group_name = data.azurerm_cosmosdb_account.main.resource_group_name
  account_name        = data.azurerm_cosmosdb_account.main.name
  database_name       = azurerm_cosmosdb_sql_database.main.name
  partition_key_path  = "/EntityId"
}
