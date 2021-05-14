
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMovies.Providers
{
    public interface IStorageTableProvider
    {
        CloudTable GetStorageTable(string tableName);
    }
}
