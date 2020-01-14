using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
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
        private ILogger _log;

        private const int DefaultPartion = 1;

        public FareTypesCosmosDb(CosmosDbSettings settings, ILogger log)
        {
            _settings = settings;
            _log = log;
        }

        private async Task<Container> Container()
        {
            if (_container == null)
            {
                try
                {
                    if (string.IsNullOrEmpty(_settings.EndpointUri))
                        throw new Exception($"Setting is missing: {nameof(_settings.EndpointUri)}");
                    if (string.IsNullOrEmpty(_settings.PrimaryKey))
                        throw new Exception($"Setting is missing: {nameof(_settings.PrimaryKey)}");
                    if (string.IsNullOrEmpty(_settings.DatabaseId))
                        throw new Exception($"Setting is missing: {nameof(_settings.DatabaseId)}");
                    if (string.IsNullOrEmpty(_settings.ContainterId))
                        throw new Exception($"Setting is missing: {nameof(_settings.ContainterId)}");

                    _client = new CosmosClient(_settings.EndpointUri, _settings.PrimaryKey);
                    _database = await _client.CreateDatabaseIfNotExistsAsync(_settings.DatabaseId);
                    _container = await _database.CreateContainerIfNotExistsAsync(new ContainerProperties(
                        _settings.ContainterId, "/Partion"));
                }
                catch (Exception ex)
                {
                    _log.LogError($"Failed to connect to CosmosDb: {ex.Message}");
                }
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

        public async Task<FareType[]> GetAll(int skip, int limit)
        {
            var container = await Container();
            return container.GetItemLinqQueryable<CosmosFareType>(true)
                .Skip(skip)
                .Take(limit)
                .Select(cf => (FareType)cf)
                .ToArray();
        }

        public async Task<bool> Upsert(FareType data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var container = await Container();
            var cosmosData = new CosmosFareType(data, DefaultPartion);
            await container.UpsertItemAsync(cosmosData);

            return true;
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
