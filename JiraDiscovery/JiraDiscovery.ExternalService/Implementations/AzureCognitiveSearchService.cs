using Azure.Search.Documents.Indexes;
using Azure;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Azure.Search.Documents;
using Microsoft.Extensions.Logging;

namespace JiraDiscovery.ExternalService.Implementations
{
    public class AzureCognitiveSearchService<T> where T: class
    {
        private readonly string serviceName = "<Put your search service NAME here>";

        private readonly string apiKey = "<Put your search service ADMIN API KEY here>";
        
        private readonly ILogger<AzureCognitiveSearchService<T>> _logger;

        public AzureCognitiveSearchService(ILogger<AzureCognitiveSearchService<T>> logger)
        {
            _logger = logger;
        }

        public async Task LoadDataIntoIndexAsync(string indexName, List<T> data)
        {
            try
            {
                Uri serviceEndpoint = new($"https://{serviceName}.search.windows.net/");

                AzureKeyCredential credential = new(apiKey);

                SearchIndexClient adminClient = new(serviceEndpoint, credential);

                FieldBuilder fieldBuilder = new();

                var searchFields = fieldBuilder.Build(typeof(T));

                var definition = new SearchIndex(indexName, searchFields);

                await adminClient.CreateOrUpdateIndexAsync(definition);

                SearchClient ingesterClient = adminClient.GetSearchClient(indexName);

                IndexDocumentsBatch<T> batch = IndexDocumentsBatch.Upload(data);

                await ingesterClient.IndexDocumentsAsync(batch).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading data into index {INDEX_NAME}", indexName);
            }
        }
    }
}
