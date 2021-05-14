using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMovies.Providers
{
    public interface IStorageQueueProvider
    {
        CloudQueue GetQueue(string tableName);
    }
}
