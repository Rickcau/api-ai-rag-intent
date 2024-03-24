// Created by: Rick Caudle
// Working: 3/23/2024
// You must create the Resource Group prior to running this command
// az group create --location westus --resource-group rg-west-aoai-demo
// az deployment group create --resource-group rg-west-aoai-demo --template-file azureAIService.bicep --parameters location=westus

param location string = resourceGroup().location
param namePrefix string = 'codewith'
var account_aoai_name = '${namePrefix}${uniqueString(resourceGroup().id)}'

resource account 'Microsoft.CognitiveServices/accounts@2023-05-01' = {
  name: account_aoai_name
  location: location
  tags: {
    codewith: 'ai-demos'
  }
  kind: 'OpenAI'
   properties: {
    customSubDomainName: account_aoai_name 
    networkAcls: {
      defaultAction: 'Allow'
      ipRules: []
      virtualNetworkRules: []
    }
    publicNetworkAccess: 'Enabled'
  }
  sku: {
    name: 'S0'
  }
}

resource accounts_name_gpt_35_turbo 'Microsoft.CognitiveServices/accounts/deployments@2023-05-01' = {
  parent: account
  name: 'gpt-35-turbo'
  properties: {
    model: {
      format: 'OpenAI'
      name: 'gpt-35-turbo'
      version: '1106'
    }
  }
   sku: {
    capacity: 100
    name: 'Standard'
  }
}
