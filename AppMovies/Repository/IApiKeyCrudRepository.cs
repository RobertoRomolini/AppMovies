using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppMovies.Repository
{
    public interface IApiKeyCrudRepository
    {
        Task<string> Get();
        Task Update(string encryptedApiKey);
    }
}
