# api-ai-rag-intent

## Summary
In this example, I implement an **Intent Recognition** pattern to detect the intent of the user’s prompt. The Chat Completion API allows you to set the **N** property for the prompt and when set it determines how many responses the LLM will provide for the user prompt in one call.  The advantage of this, is that it allows the LLM in one call to attempt to determine the **Intent** multiple times.  The reason this is important is because the LLM can incorrectly predict the **Intent**. 

Based on various conversations I have had the Semantic Kernel team, I was advised that I should take advantage of the ResultsPerPrompt property, which is exposed via **OpenAIPromptExecutionSettings** in the Semantic Kernel.  When using the ResultsPerPrompt and setting it to anything greater than 1, you must use **IChatCompletionService.GetChatMessageContentsAsync** and not **IChatCOmpletionService.GetChatMessageContentAsync**.

By using this pattern, it provides the following benefits:
•	Allows you to only make calls to your AI Search Service when the **Intent** of the prompt is document search related.  In this example, the service provides two features, the ability to provide details about documents we have indexed and the ability to perform SubGraph Queries against the UniswapV3 SubGraph Endpoint.
•	Allows you to branch based on **Intent** and write more efficient code
•	Reduces the number of Tokens being consumed and allows you to only call the AI endpoint when needed
•	Using the ResultsPerPrompt allows you to determine the quorum based on 3 results, even if the quorum is incorrect you still minimize the number of times the AI endpoint will be unnecessarily called. 
•	Allows you to get 3 results from the LLM with just one call to the endpoint.  

In my testing the LLM seems to perform best with a Temperature of 0.5 and a ResultsPerPrompt of 3.  LLMs can incorrectly predict the **Intent** and this is why you want to ask it for 3 results, as this allows the LLM to reevaluate the prediction.   

## Goal
The goal is to demostrate the use of **Intent Recognition** in conjunction with RAG (Retreval Augmentation Generation) using Semantic Kernel (1.4.0) an Isolated Azure Function.  It leverages all the common patterns e.g. dependecy injection, SK Plugins/Functions, AutoInvoke etc. 

## Patterns 
- RAG (Retreval Augmentation Generation) using Semantic + Vector Seach
- Intent Recognition
- SK Functions & Plugins

## Technologies
- Azure Function (Isolated) as REST Endpoint
- Semantic Kernel (1.4.0)
- Azure AI Search (Semantic + Vector)
- UniswapV3 SubGraph API
- Dependency Injection
- Azure Open AI

## Bicep Deployment and Scripts
If you would like to deploy the resouces for this example you can use the .bicep templates, just read the notess in the files to understand how to use them.

You can use the UploadDocument.ps1 to upload the employee_handbook.pdf to the Storage Account Container.  The CreateIndex.ps1 script is not complete yet so you will need to refactor it to get it working.  For now, you can simply use the **Import and Vectorize** option from the Azure Portal to build the index that will be used in this example.

### Request Body
The function expects you to pass a JSON body with the following information:

        ~~~
              {
                 "userId": "stevesmith@contoso.com",
                 "sessionId": "12345678",
                 "tenantId": "00001",
                 "prompt": "Can you tell what my healtcare benefits are for Northwinds"
              }
        ~~~

The client that is calling the Function will pass these values in and later they can be used to store/retreive prompt history. In a future version I will add support to storing/retreving of ChatHistory using CosmosDB.

## Steps to deploy the Azure Resouces needed for this example
### Create an Azure Resouce Group
1. Open a Terminal Windows in Visual Studio and run the following command
   
   ~~~
       az group create --name AIRagIntentResourceGroup --location westus
   ~~~

2. Open the .bicep files and read the instructions for details on how to deploy the resouces using the templates.
  