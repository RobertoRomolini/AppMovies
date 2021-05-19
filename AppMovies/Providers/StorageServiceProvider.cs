using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMovies.Providers
{
    public class StorageServiceProvider : IStorageServiceProvider
    {
        public CloudTable GetStorageTable(string tableName)
        {
            Microsoft.Azure.Cosmos.Table.CloudStorageAccount storageAccount = Microsoft.Azure.Cosmos.Table.CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(tableName);
            table.CreateIfNotExists();
            return table;
        }
        public CloudQueue GetQueueTable(string tableName)
        {
            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(tableName);
            queue.CreateIfNotExistsAsync();
            return queue;
        }
        public CloudBlobContainer GetBlobContainer(string tableName)
        {
            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(tableName);
            container.CreateIfNotExistsAsync();
            return container;
        }
        public BlobContainerClient GetContainerClient(string tableName)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient("UseDevelopmentStorage=true");
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(tableName);
            return containerClient;
        }
    }
}
