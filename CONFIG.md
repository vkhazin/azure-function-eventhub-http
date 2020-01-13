# Configuration
## SQL Server

1. Configure connectivity settings using the following instruction: https://docs.microsoft.com/en-us/azure/virtual-machines/windows/sql/virtual-machines-windows-sql-connect#change Set `SQL Connectivity` to `Public`, `SQL Authentication` to `Enable`, set login and password. 
2. Construct connection string: `Server=<your_vm>.cloudapp.azure.com;Integrated Security=false;User ID=<login_name>;Password=<your_password>`

## Azure
### Create resource group and storage account

Replace `<resource_group>`, `<storage_name>` (and `location`, if necessary) with your values: 

    az group create --name <resource_group> --location northeurope
    az storage account create --name <storage_name> --location northeurope --resource-group <resource_group> --sku Standard_LRS
    
### Create functions

Replace `<funcHttp>` and `<funcEventHub>` with your values:

    az functionapp create --resource-group <resource_group> --consumption-plan-location northeurope \
    --name <funcHttp> --storage-account <storage_name> --runtime dotnet

    az functionapp create --resource-group <resource_group> --consumption-plan-location northeurope \
    --name <funcEventHub> --storage-account <storage_name> --runtime dotnet

## Settings

Full set of settings is required for both functions:

1. Proceed to Function App from Azure Portal
2. Select function name
3. Click **Configuration** under Configured features on the Overview tab
4. Use **New Application Setting** button to add settings

* `AzureWebJobsStorage`: storage account connection string, required for event hub trigger. How to get it from Azure Portal: https://docs.microsoft.com/en-us/azure/storage/common/storage-configure-connection-string#view-and-copy-a-connection-string
* `SqlConnectionString`: SQL Server connection string constructed above.
* `CosmosEndpointUri`: Cosmos Db account endpoint Uri (https://docs.microsoft.com/en-us/azure/cosmos-db/secure-access-to-data#master-keys)
* `CosmosPrimaryKey`: Cosmos Db Primary key
* `CosmosDatabaseId` and `CosmosContainterId`: database and container ids
* `EventHubConnectionString`: event hub connection string. How to obtain: https://docs.microsoft.com/en-us/azure/event-hubs/event-hubs-get-connection-string#get-connection-string-from-the-portal

These settings should be also added to `local.settings.json` file to run app locally.

## Deploy

```
func azure functionapp publish <funcHttp>
func azure functionapp publish <funcEventHub>
```

## Run

    cd azureFareTypes
    func start --build
