# TBD: This script is incomplete and needs work so feel free to modify

# Authenticate to Azure (you might need to log in if you haven't already)
Connect-AzAccount

# Variables
$resourceGroupName = "your_resource_group_name"
$searchServiceName = "your_search_service_name"
$indexName = "your_index_name"
$storageAccountName = "your_storage_account_name"
$containerName = "documents"
$blobName = "employee_handbook.pdf"
$subscriptionKey = "your_subscription_key"
$openAIEndpoint = "your_url_to_aoai_endpoint"


# Function to call Azure OpenAI API for text embedding
function Invoke-OpenAIAPITextEmbedding {
    param(
        [string]$text
    )
    $headers = @{
        "Authorization" = "Bearer $subscriptionKey"
        "Content-Type" = "application/json"
    }
    $body = @{
        prompt = $text
    } | ConvertTo-Json
    $response = Invoke-RestMethod -Uri $openAIEndpoint -Method Post -Headers $headers -Body $body
    return $response.choices[0].text
}

# Get the key for the Azure Cognitive Search service
$searchService = Get-AzSearchService -ResourceGroupName $resourceGroupName -Name $searchServiceName
$searchServiceKey = Get-AzSearchServiceAdminKey -ResourceGroupName $resourceGroupName -ServiceName $searchServiceName

# Get the SAS token for the Azure Blob Storage container
$sasToken = New-AzStorageContainerSASToken -Context $ctx -Name $containerName -Permission "r" -ExpiryTime (Get-Date).AddHours(1)

# Get the content of the blob
$blobUrl = "https://$storageAccountName.blob.core.windows.net/$containerName/$blobName$sasToken"
$content = Invoke-RestMethod -Uri $blobUrl -Method Get

# Generate vectorized representation using Azure OpenAI
$vectorizedText = Invoke-OpenAIAPITextEmbedding -text $content

# Index the vectorized representation into Azure Cognitive Search
$searchIndex = @{
    value = @(
        @{
            "@search.action" = "mergeOrUpload"
            "id" = "unique_blob_id" # Provide a unique identifier for each blob
            "content" = $content
            "vectorizedContent" = $vectorizedText
        }
    )
} | ConvertTo-Json

$searchEndpoint = "https://$searchServiceName.search.windows.net/indexes/$indexName/docs/index?api-version=2020-06-30"
Invoke-RestMethod -Uri $searchEndpoint -Method Post -Headers @{ "api-key" = $searchServiceKey.primaryKey; "Content-Type" = "application/json" } -Body $searchIndex