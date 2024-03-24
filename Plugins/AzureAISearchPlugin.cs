using api_ai_rag_intent.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using System.ComponentModel;


namespace api_ai_rag_intent.Plugins
{
    public class AzureAISearchPlugin
    {
        private readonly IAzureAISearchService _searchService;
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        private readonly ITextEmbeddingGenerationService _textEmbeddingGenerationService;


        public AzureAISearchPlugin(ITextEmbeddingGenerationService textEmbeddingGenerationService, IAzureAISearchService searchService)
        {
            this._textEmbeddingGenerationService = textEmbeddingGenerationService;
            this._searchService = searchService;
        }
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.


        [KernelFunction]
        [Description("When a user asks question about a policy, or how to do something, or uses any acronym, or how someone is, use this function to perform the search")]
        public async Task<string> SimpleVectorSearchAsync(
           string query,
           string index,
           List<string>? searchFields = null,
           CancellationToken cancellationToken = default)
        {
            // Convert string query to vector
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            ReadOnlyMemory<float> embedding = await this._textEmbeddingGenerationService.GenerateEmbeddingAsync(query, cancellationToken: cancellationToken);
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            // Perform simple search
            return await this._searchService.SimpleVectorSearchAsync(embedding, query, index) ?? string.Empty;
        }

        [KernelFunction]
        [Description("FetchFromManuals: When a user asks question about a policy, or how to do something, or uses any acronym, or who someone is, use this function to perform the search")]
        public async Task<string> SemanticHybridSearchAsync(
          string query,
          string index,
          string semanticconfigname,
          List<string>? searchFields = null,
          CancellationToken cancellationToken = default)
        {
            // Convert string query to vector
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            ReadOnlyMemory<float> embedding = await this._textEmbeddingGenerationService.GenerateEmbeddingAsync(query, cancellationToken: cancellationToken);
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            // Perform simple search
            return await this._searchService.SemanticHybridSearchAsync(embedding, query, index, semanticconfigname) ?? string.Empty;
        }


    }
}
