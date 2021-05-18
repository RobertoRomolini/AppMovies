using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMovies.Entities
{
    public class ApiKeyEntity : TableEntity
    {
        public ApiKeyEntity()
        {
        }
        public string ApiKey { set; get; }
    }
}
