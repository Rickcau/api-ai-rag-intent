using api_ai_rag_intent.Models;
using api_ai_rag_intent.Util;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Net;
using System.Text.Json;

namespace api_ai_rag_intent.Functions
{
    public class ChatProvider
    {
        private readonly ILogger<ChatProvider> _logger;
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chat;
        private readonly ChatHistory _chatHistory;
        private readonly string _aiSearchIndex = Helper.GetEnvironmentVariable("AISearchIndex");
        private readonly string _semanticSearchConfigName = Helper.GetEnvironmentVariable("AISearchSemanticConfigName");


        public ChatProvider(ILogger<ChatProvider> logger, Kernel kernel, IChatCompletionService chat, ChatHistory chatHistory)
        {
            _logger = logger;
            _kernel = kernel;
            _chat = chat;
            _chatHistory = chatHistory;
        }

        [Function("ChatProvider")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            // Request body example:
            /*
                {
                    "userId": "stevesmith@contoso.com",
                    "sessionId": "12345678",
                    "prompt": "Hello, What can you do for me?"
                }
            */
            // Example querys for GraphQL:
            Console.WriteLine("Example 1: top 10 active pools");
            Console.WriteLine("Example 2: Retrieve 10 most liquid pools");

            _chatHistory.Clear();
            _logger.LogInformation("C# HTTP SentimentAnalysis trigger function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var chatRequest = JsonSerializer.Deserialize<ChatProviderRequest>(requestBody);
            if (chatRequest == null || chatRequest.userId == null || chatRequest.sessionId == null || chatRequest.tenantId == null || chatRequest.prompt == null)
            {
                throw new ArgumentNullException("Please check your request body, you are missing required data.");
            }

            var intent = await Util.Intent.GetIntent(_chat, chatRequest.prompt);
            // The purpose of using an Intent pattern is to allow you to make decisions about how you want to invoke the LLM
            // In the case of RAG, if you can detect the user intent is to related to searching documents, then you can only perform that action when the intent is to search documents
            // this allows you to reduce the token useage and save you TPM and dollars
            switch (intent)
            {
                case "documents":
                    {
                        Console.WriteLine("Intent: documents");
                        Helper.GetEnvironmentVariable("ApiKey");
                        var function = _kernel.Plugins.GetFunction("AzureAISearchPlugin", "SemanticHybridSearch");
                        var content = (await _kernel.InvokeAsync(function, new() { ["query"] = chatRequest.prompt, ["index"] = _aiSearchIndex, ["semanticconfigname"] = _semanticSearchConfigName })).ToString();
                        _chatHistory.AddUserMessage("If [Title:] is included in the content you are summarizing please include that at the end of your summary.");
                        _chatHistory.AddUserMessage(content);
                        _chatHistory.AddUserMessage(chatRequest.prompt);
                        break;
                    }
                case "graphql":
                    {
                        // Now that I know the intent of the question is graphql related, I could just call the plugin directly
                        // but, since I have AutoInvokeKernelFunctions enabled I can just let SK detect that it needs to call the funciton and let it do it.
                        // Now, it would be more performant to just call it directly as their is additional overhead with SK searching the plugin collection etc
                        _chatHistory.AddUserMessage(chatRequest.prompt);
                        Console.WriteLine("Intent: graphql");
                        break;
                    }
                case "not_found":
                    {
                        _chatHistory.AddUserMessage("Simply reponse in a polite way that the request is not related to documents or Subgraph Queries so you are unable to help.");
                        Console.WriteLine("Intent: not_found");
                        break;
                    }

            }
            ChatMessageContent? result = null;

            if (intent != "not_found")
            {
                result = await _chat.GetChatMessageContentAsync(
                    _chatHistory,
                    executionSettings: new OpenAIPromptExecutionSettings { Temperature = 0.8, TopP = 0.0, ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions },
                    kernel: _kernel);
            }

            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            try
            {
                var notfound = @$"Your question isn't related to materials I have indexed or related to subgraph queries, so I am unable to help.  
                             Please ask a question related to documents I have indexed or something like find the top 10 active pools.";
                await response.WriteStringAsync(result?.Content ?? notfound);
            }
            catch (Exception ex)
            {
                // Log exception details here
                Console.WriteLine(ex.ToString());
                throw; // Re-throw the exception to propagate it further
            }

            return response;
        }
    }
}
