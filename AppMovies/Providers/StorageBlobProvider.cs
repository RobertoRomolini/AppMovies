using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMovies.Providers
{
    public class StorageBlobProvider : IStorageBlobProvider
    {
        public CloudBlobContainer GetBlobContainer(string tableName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(tableName);
            container.CreateIfNotExistsAsync();
            return container;
        }
    }
}
