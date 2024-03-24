using api_ai_rag_intent.Interfaces;
using api_ai_rag_intent.Util;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using System.Text;
using System.Text.Json.Serialization;

namespace api_ai_rag_intent.Services
{
    internal class IndexSchema
    {
        [JsonPropertyName("chunk_id")]
        public string ChunkId { get; set; } = string.Empty;

        [JsonPropertyName("parent_id")]
        public string ParentId { get; set; } = string.Empty;

        [JsonPropertyName("chunk")]
        public string Chunk { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("vector")]
        public ReadOnlyMemory<float> Vector { get; set; }
    }

    internal class AzureAISearchService : IAzureAISearchService
    {
        private readonly List<string> _defaultVectorFields = new() { "vector" };

        private readonly SearchIndexClient _indexClient;

        public AzureAISearchService(SearchIndexClient indexClient)
        {
            this._indexClient = indexClient;
        }

        public async Task<string> SimpleVectorSearchAsync(ReadOnlyMemory<float> embedding, string query, string index, int k = 3)
        {
            StringBuilder content = new StringBuilder();

            // Get client for search operations
            SearchClient searchClient = this._indexClient.GetSearchClient(index);

            // Perform the vector similarity search  
            var searchOptions = new SearchOptions
            {
                VectorSearch = new()
                {
                    Queries = { new VectorizedQuery(embedding.ToArray()) { KNearestNeighborsCount = k, Fields = { "vector" } } }
                },
                Size = k,
                Select = { "title", "chunk" },
            };

            SearchResults<SearchDocument> response;
            try
            {
                response = await searchClient.SearchAsync<SearchDocument>(query, searchOptions);

            }
            catch (Exception ex)
            {
                // Log exception details here
                Console.WriteLine(ex.Message);
                throw; // Re-throw the exception to propagate it further
            }

            //SearchResults<SearchDocument> response = await searchClient.SearchAsync<SearchDocument>(query, searchOptions);

            int count = 0;
            await foreach (SearchResult<SearchDocument> result in response.GetResultsAsync())
            {
                count++;
                Console.WriteLine($"Title: {result.Document["title"]}");
                Console.WriteLine($"Score: {result.Score}\n");
                Console.WriteLine($"Content: {result.Document["chunk"]}");
                content.AppendLine(result.Document["chunk"].ToString() ?? string.Empty);
            }
            Console.WriteLine($"Total Results: {count}");

            return content.ToString();
        }

        public async Task<string> SemanticHybridSearchAsync(ReadOnlyMemory<float> embedding, string query, string index, string semanticconfigname, int k = 3)
        {
            // This is a combination of a semantic and vector search
            StringBuilder content = new StringBuilder();

            try
            {
                SearchClient searchClient = this._indexClient.GetSearchClient(index);

                var searchOptions = new SearchOptions
                {
                    VectorSearch = new()
                    {
                        Queries = { new VectorizedQuery(embedding.ToArray()) { KNearestNeighborsCount = k, Fields = { "vector" } } }
                    },
                    SemanticSearch = new()
                    {
                        SemanticConfigurationName = semanticconfigname,
                        QueryCaption = new(QueryCaptionType.Extractive),
                        QueryAnswer = new(QueryAnswerType.Extractive),
                    },
                    QueryType = SearchQueryType.Semantic,
                    Select = { "title", "chunk", },

                };

                // Perform search request
                Response<SearchResults<IndexSchema>> response = await searchClient.SearchAsync<IndexSchema>(query, searchOptions);
                List<IndexSchema> results = new();
                // Collect search results
                await foreach (SearchResult<IndexSchema> result in response.Value.GetResultsAsync())
                {
                    if (result.SemanticSearch.RerankerScore > 0.5 || result.Score > 0.03)
                    {
                        results.Add(result.Document);
                        //content.AppendLine(result.Document.ToString());
                    }
                }
                var sortedResults = results
                    .OrderByDescending(result => result.ChunkId)
                    .Take(3)
                    .ToList();
                foreach (var result in sortedResults)
                {
                    content.AppendLine("documents:");
                    content.AppendLine(result.Chunk);
                    content.AppendLine("Title: " + result.Title);
                }

            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Total Results: 0");
            }
            return content.ToString();
        }
    }
}
