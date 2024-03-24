// storageAccount.bicep
// Created By: Rick Caudle
// Working: 3/23/23
// You must create the Resource Group prior to running this command
// az group create --location westus --resource-group rg-west-aoai-demo
// az deployment group create --resource-group rg-west-aoai-demo --template-file searchService.bicep --parameters location=westus


param location string = resourceGroup().location
param namePrefix string = 'codewithrag'
var storage_acct_name = '${namePrefix}${uniqueString(resourceGroup().id)}'


resource storageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' = {
  name: storage_acct_name // Set your desired storage account name here
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}

// Create blob service
resource blobServices 'Microsoft.Storage/storageAccounts/blobServices@2023-01-01' = {
    name: 'default'
    parent: storageAccount
}

resource storageContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-04-01' = {
  parent: blobServices
  name: 'documents'
  properties: {
    publicAccess: 'None'
  }
}

