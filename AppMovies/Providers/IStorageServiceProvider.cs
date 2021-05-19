using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMovies.Providers
{
    public interface IStorageServiceProvider
    {
        CloudBlobContainer GetBlobContainer(string tableName);
        CloudQueue GetQueueTable(string tableName);
        CloudTable GetStorageTable(string tableName);
        BlobContainerClient GetContainerClient(string tableName);
    }
}
