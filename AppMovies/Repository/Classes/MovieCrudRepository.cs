using AppMovies.Entities;
using AppMovies.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AppMovies.Repository
{
    class MovieCrudRepository : ITableCrudRepository<MovieEntity>
    {
        private readonly CloudTable _table;

        public MovieCrudRepository(IStorageServiceProvider storageTableProvider)
        {
            _table = storageTableProvider.GetStorageTable("movies");
        }
        public async Task<IEnumerable<MovieEntity>> GetAll(QueryString parameters)
        {
            var collection = HttpUtility.ParseQueryString(parameters.ToString());

            var partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "movie");
            
            string allFilters = "";
            foreach (string key in collection.AllKeys)
            {
                string keyValue = collection[key];

                // Count the number of digits at the end of the string
                int numbersAtEnd = 0;
                int position = 1;
                while (keyValue.Length >= position && char.IsDigit(keyValue[keyValue.Length - position]))
                {
                    numbersAtEnd++;
                    position++;
                }

                string rowKeyRangeEnd = "";

                // If the string contains numbers at the end, the number is incremented as a number, not as a string
                if (numbersAtEnd > 0)
                {
                    int endNumber = Int32.Parse(keyValue.Substring(keyValue.Length - numbersAtEnd));
                    string startString = keyValue.Substring(0, keyValue.Length - numbersAtEnd);
                    rowKeyRangeEnd = startString + (endNumber + 1).ToString();
                }
                else
                {
                    rowKeyRangeEnd = keyValue.Substring(0, keyValue.Length - 1) + (char)(keyValue.Last() + 1);
                }

                var likeFilter = TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition(key, QueryComparisons.GreaterThan, keyValue),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition(key, QueryComparisons.LessThan, rowKeyRangeEnd));
                
                var likePartitionFilter = TableQuery.CombineFilters(partitionKeyFilter, TableOperators.And, likeFilter);
                var equalPartitionFilter = TableQuery.CombineFilters(partitionKeyFilter, TableOperators.And,
                    TableQuery.GenerateFilterCondition(key, QueryComparisons.Equal, keyValue));

                var likeEqualFilter = TableQuery.CombineFilters(likePartitionFilter, TableOperators.Or, equalPartitionFilter);

                if (allFilters != "")
                {
                    allFilters = TableQuery.CombineFilters(allFilters, TableOperators.And, likeEqualFilter);
                }
                else
                {
                    allFilters = likeEqualFilter;
                }
            }

            var query = new TableQuery<MovieEntity>().Where(allFilters);

            TableContinuationToken token = null;
            var entities = new List<MovieEntity>();
            do
            {
                var queryResult = await _table.ExecuteQuerySegmentedAsync(query, token).ConfigureAwait(false);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            }
            while (token != null);

            return entities;
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

        public async Task<MovieEntity> Add(TableEntity entity)
        {
            TableOperation insertOperation = TableOperation.Insert(entity);
            TableResult result = await _table.ExecuteAsync(insertOperation);
            return (MovieEntity)result.Result;
        }

        public async Task<MovieEntity> Update(TableEntity entity)
        {
            TableOperation replaceOperation = TableOperation.Replace(entity);
            TableResult result = await _table.ExecuteAsync(replaceOperation);
            return (MovieEntity) result.Result;
        }

        public async Task Delete(string rowKey)
        {
            TableOperation deleteOperation = TableOperation.Delete(
                new MovieEntity()
                {
                    RowKey = rowKey ,
                    PartitionKey = "movie",
                    ETag = "*"
                }
            );
            TableResult result = await _table.ExecuteAsync(deleteOperation);
        }
    }
}

