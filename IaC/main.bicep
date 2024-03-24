// Created By: Rick Caudle
// Working: 3/23/24

// This deployment main.Bicep file is intented to be used to deploy the necesscary response for the api-ai-rag-intent example.
// It is intented to be very simple and the goal is to quickly get the resouces deployed so you can start experimenting 
// Feel free to use this as an example of how to deploy resources using IaC

// Modify the deploy.bat and then run it or you can run az deployment against each . bicep file.
// It does not publish the Azure Function, it only deploys the Resouces the Code has dependencies on.  If you would like to publish the Azure Function, you can use the publish feature in Visual Studio.

// You must create the Resource Group prior to running this command
// az group create --location westus --resource-group rg-west-aoai-demo
// az deployment group create --resource-group rg-west-aoai-demo --template-file main.bicep --parameters location=westus

param location string = resourceGroup().location
var examplePrefix = 'codewith-'
var ragStorageAccountName = '${examplePrefix}${uniqueString(resourceGroup().id)}'
var uniqueAzureServicesName = '${examplePrefix}${uniqueString(resourceGroup().id)}'
var uniqueSearchServicesName = '${examplePrefix}${uniqueString(resourceGroup().id)}'

module stgModule 'storageAccount.bicep' = {
  name: ragStorageAccountName
  params: {
    location: location
  }
}
module azServicesModule 'azureAIService.bicep' = {
  name: uniqueAzureServicesName
  params: {
    location: location
  }
}

module azSearchModule 'searchService.bicep' = {
  name: uniqueSearchServicesName
  params: {
    location: location
  }
}
