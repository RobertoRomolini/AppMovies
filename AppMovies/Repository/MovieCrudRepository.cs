using AppMovies.Entities;
using AppMovies.Providers;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppMovies.Repository
{
    class MovieCrudRepository : ITableCrudRepository<MovieEntity>
    {
        private readonly CloudTable _table;

        public MovieCrudRepository(IStorageTableProvider storageTableProvider)
        {
            _table = storageTableProvider.GetStorageTable("movies");
        }

        public async Task<MovieEntity> Get(string rowKey)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<MovieEntity>("movie", rowKey);
            TableResult result = await _table.ExecuteAsync(retrieveOperation).ConfigureAwait(false);
            return (MovieEntity)result.Result;
        }

        public async Task<MovieEntity> AddOrUpdate(TableEntity entity)
        {
            TableOperation insertOperation = TableOperation.InsertOrReplace(entity);
            TableResult result = await _table.ExecuteAsync(insertOperation);
            return (MovieEntity)result.Result;
        }

        public async Task Delete(string rowKey)
        {
            TableOperation deleteOperation = TableOperation.Delete(
                new MovieEntity(rowKey)
                {
                    PartitionKey = "movie",
                    ETag = "*"
                }
            );
            TableResult result = await _table.ExecuteAsync(deleteOperation);
        }
    }
}

