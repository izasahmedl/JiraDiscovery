using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;

namespace JiraDiscovery.ExternalService.Implementations
{
    public class CosmosDbService
    {
        private readonly CosmosClient _cosmosClient;
        private readonly string _databaseId;
        private readonly string _containerId;

        public CosmosDbService(string connectionString, string databaseId, string containerId)
        {
            _cosmosClient = new CosmosClientBuilder(connectionString).Build();
            _databaseId = databaseId;
            _containerId = containerId;
        }

        public async Task CreateItemAsync<T>(T item)
        {
            var container = _cosmosClient.GetContainer(_databaseId, _containerId);
            await container.UpsertItemAsync(item);
        }

        public async Task<T> ReadItemAsync<T>(string id, string partitionKey)
        {
            var container = _cosmosClient.GetContainer(_databaseId, _containerId);
            try
            {
                var response = await container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return default!;
            }
        }

        public async Task UpdateItemAsync<T>(string id, string partitionKey, T updatedItem)
        {
            var container = _cosmosClient.GetContainer(_databaseId, _containerId);
            await container.ReplaceItemAsync(updatedItem, id, new PartitionKey(partitionKey));
        }

        public async Task DeleteItemAsync(string id, string partitionKey)
        {
            var container = _cosmosClient.GetContainer(_databaseId, _containerId);
            await container.DeleteItemAsync<object>(id, new PartitionKey(partitionKey));
        }

        public async Task<IEnumerable<T>> QueryItemsAsync<T>(string query)
        {
            var container = _cosmosClient.GetContainer(_databaseId, _containerId);
            var queryDefinition = new QueryDefinition(query);
            var queryResultSetIterator = container.GetItemQueryIterator<T>(queryDefinition);

            var results = new List<T>();
            while (queryResultSetIterator.HasMoreResults)
            {
                var response = await queryResultSetIterator.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }
    }
}
