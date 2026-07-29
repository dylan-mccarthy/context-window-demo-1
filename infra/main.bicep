targetScope = 'resourceGroup'

@description('Short prefix used for resource names.')
param namePrefix string = 'payments'

@allowed([
  'dev'
  'test'
  'prod'
])
param environment string = 'dev'

@description('Team accountable for the deployed resources.')
param owner string

param location string = resourceGroup().location

var tags = {
  owner: owner
  service: 'payments-api'
  environment: environment
}

module monitoring 'modules/monitoring.bicep' = {
  params: {
    namePrefix: namePrefix
    environment: environment
    location: location
    tags: tags
  }
}

module appService 'modules/app-service.bicep' = {
  params: {
    namePrefix: namePrefix
    environment: environment
    location: location
    tags: tags
    applicationInsightsConnectionString: monitoring.outputs.applicationInsightsConnectionString
    logAnalyticsWorkspaceId: monitoring.outputs.logAnalyticsWorkspaceId
  }
}

output appServiceName string = appService.outputs.appServiceName
output appServiceHostName string = appService.outputs.defaultHostName
