az group create --location westus --resource-group rg-api-ai-rag-intent
echo Running the deployment
az deployment group create --resource-group rg-api-ai-rag-intent --template-file main.bicep --parameters location=westus

