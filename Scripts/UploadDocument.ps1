# Working: 3/24/24
# Upload Employee Handbook
# Set your Azure Storage account details
$storageAccountName = "your_storage_account_name"
$containerName = "documents"
$resourceGroup = "your_resource_group_name"
$localFilePath = "..\Samples\employee_handbook.pdf"

# Authenticate to Azure
Connect-AzAccount

# Get the storage account context
$storageContext = (Get-AzStorageAccount -ResourceGroupName $resourceGroup -Name $storageAccountName).Context

# Upload the file to Azure Blob Storage
Set-AzStorageBlobContent -File $localFilePath -Container $containerName -Blob (Split-Path $localFilePath -Leaf) -Context $storageContext