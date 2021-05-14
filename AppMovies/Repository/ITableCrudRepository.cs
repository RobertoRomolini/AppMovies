using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppMovies.Repository
{
    public interface ITableCrudRepository<TEntity>
    {
        //Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity> Get(string rowKey);
        Task<TEntity> AddOrUpdate(TableEntity entity);
        //Task<TEntity> Update(TableEntity entity);
        Task Delete(string rowKey);
    }
}
