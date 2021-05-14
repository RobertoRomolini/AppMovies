using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMovies.Providers
{
    public class StorageQueueProvider : IStorageQueueProvider
    {
        public CloudQueue GetQueue(string tableName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(tableName);
            queue.CreateIfNotExistsAsync();
            return queue;
        }
    }
}
