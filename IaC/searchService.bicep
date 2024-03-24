// Created by: Rick Caudle
// Working: 3/23/2024
// You must create the RG prior to running the deployment command
// az group create --location westus --resource-group rg-west-aoai-demo
// az deployment group create --resource-group rg-west-aoai-demo --template-file searchService.bicep --parameters location=westus

param location string = resourceGroup().location
param namePrefix string = 'codewith'
var search_name = '${namePrefix}${uniqueString(resourceGroup().id)}'

var searchIdentityProvider = {
  type: 'SystemAssigned'
}

resource search 'Microsoft.Search/searchServices@2021-04-01-preview' = {
  name: search_name
  location: location
  tags: {
    codewith: 'ai-demos'
  }
  identity: searchIdentityProvider
  properties: {
      authOptions: {
      apiKeyOnly: {}
    }
   disableLocalAuth: false
    encryptionWithCmk: {
      enforcement: 'Unspecified'
    }
    hostingMode: 'default'
    networkRuleSet: {
      ipRules: []
    }
    partitionCount: 1
    publicNetworkAccess: 'enabled'
    replicaCount: 1
    semanticSearch: 'standard'
  }
  sku: {
    name: 'standard'
  }
}
