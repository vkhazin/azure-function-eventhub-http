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
    public class FareTypesCosmosDb : IFareTypesCosmosDb
    {
        private CosmosDbSettings _settings;
        private CosmosClient _client;
        private Database _database;
        private Container _container;

        private const int DefaultPartion = 1;

        public FareTypesCosmosDb(CosmosDbSettings settings)
        {
            _settings = settings;
        }

        private async Task<Container> Container()
        {
            if (_container == null)
            {
                _client = new CosmosClient(_settings.EndpointUri, _settings.PrimaryKey);
                _database = await _client.CreateDatabaseIfNotExistsAsync(_settings.DatabaseId);
                _container = await _database.CreateContainerIfNotExistsAsync(new ContainerProperties(
                    _settings.ContainterId, "/Partion"));
            }
            return _container;
        }

        public async Task<FareType> Get(int id) => await Get(id.ToString());

        private async Task<FareType> Get(string id)
        {
            try
            {
                var container = await Container();
                var itemResponse = await container.ReadItemAsync<CosmosFareType>(id, new PartitionKey(DefaultPartion));
                return itemResponse.Resource.ToFareType();
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<FareType[]> GetAll()
        {
            var container = await Container();
            return container.GetItemLinqQueryable<CosmosFareType>(true)
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
                var container = await Container();
                var cosmosData = new CosmosFareType(data, DefaultPartion);
                await container.CreateItemAsync(cosmosData, new PartitionKey(DefaultPartion));
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
                var container = await Container();
                var cosmosData = new CosmosFareType(data, DefaultPartion);
                await container.ReplaceItemAsync(cosmosData, id);
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
                var container = await Container();
                await container.DeleteItemAsync<CosmosFareType>(id, new PartitionKey(DefaultPartion));
            }
            return existingItem != null;
        }
    }
}
