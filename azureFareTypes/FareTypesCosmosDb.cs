using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace azureFareTypes
{
    public class FareTypesCosmosDb
    {
        private CosmosClient _client;
        private Database _database;
        private Container _container;

        private const int DefaultPartion = 1;

        public async Task Init()
        {
            _client = new CosmosClient(
                Environment.GetEnvironmentVariable("CosmosEndpointUri"),
                Environment.GetEnvironmentVariable("CosmosPrimaryKey"));

            _database = await _client.CreateDatabaseIfNotExistsAsync(
                Environment.GetEnvironmentVariable("CosmosDatabaseId"));
            _container = await _database.CreateContainerIfNotExistsAsync(new ContainerProperties(
                Environment.GetEnvironmentVariable("CosmosContainterId"), "/Partion"));
        }

        public async Task<FareType> Get(int id) => await Get(id.ToString());

        public async Task<FareType> Get(string id)
        {
            try
            {
                var itemResponse = await _container.ReadItemAsync<CosmosFareType>(id, new PartitionKey(DefaultPartion));
                return itemResponse.Resource.ToFareType();
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public FareType[] GetAll()
        {
            return _container.GetItemLinqQueryable<CosmosFareType>(true)
                .Select(cf => (FareType)cf)
                .ToArray();
        }

        public async Task<bool> Insert(FareType data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var existingItem = await Get(data.FareTypeId);
            if (existingItem == null)
            {
                var cosmosData = new CosmosFareType(data, DefaultPartion);
                await _container.CreateItemAsync(cosmosData, new PartitionKey(DefaultPartion));
            }
            return existingItem == null;
        }

        public async Task<bool> Update(string id, FareType data)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            var existingItem = await Get(id);
            if (existingItem != null)
            {
                var cosmosData = new CosmosFareType(data, DefaultPartion);
                await _container.ReplaceItemAsync(cosmosData, id);
            }
            return existingItem != null;
        }

        public async Task<bool> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            var existingItem = await Get(id);
            if (existingItem != null)
            {
                await _container.DeleteItemAsync<CosmosFareType>(id, new PartitionKey(DefaultPartion));
            }
            return existingItem != null;
        }
    }
}
