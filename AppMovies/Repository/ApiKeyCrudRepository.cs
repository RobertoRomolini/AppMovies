using AppMovies.Entities;
using AppMovies.Providers;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppMovies.Repository
{
    public class ApiKeyCrudRepository : IApiKeyCrudRepository
    {
        private readonly CloudTable _table;

        public ApiKeyCrudRepository(IStorageTableProvider storageTableProvider)
        {
            _table = storageTableProvider.GetStorageTable("apikey");
        }

        public async Task<string> Get()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<ApiKeyEntity>("apikey", "apikey");
            TableResult result = await _table.ExecuteAsync(retrieveOperation);
            ApiKeyEntity apiKeyEntity = (ApiKeyEntity)result.Result;
            return apiKeyEntity.ApiKey;
        }

        public async Task Update(string encryptedApiKey)
        {
            TableOperation replaceOperation = TableOperation.Replace(new ApiKeyEntity()
            {
                PartitionKey = "apikey",
                RowKey = "apikey",
                ETag = "*",
                ApiKey = encryptedApiKey
            });
            TableResult result = await _table.ExecuteAsync(replaceOperation);
        }
    }
}
