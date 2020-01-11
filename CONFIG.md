# Configuration

The following settings should be added to Azure Function App or to the `local.settings.json` file to run app locally.

## Common

`AzureWebJobsStorage`: storage account connection string, required for event hub trigger.

## SQL Server

`SqlConnectionString`: SQL Server connection string.

## Cosmos Db

`CosmosEndpointUri`: Cosmos Db account endpoint Uri
`CosmosPrimaryKey`: Cosmos Db primary key
`CosmosDatabaseId` and `CosmosContainterId`: database and container ids


## Event Hub

`EventHubConnectionString`: event hub connection string


# Run

    cd azureFareTypes
    func start --build