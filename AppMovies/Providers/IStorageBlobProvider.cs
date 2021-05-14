using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMovies.Providers
{
    public interface IStorageBlobProvider
    {
        CloudBlobContainer GetBlobContainer(string tableName);
    }
}
