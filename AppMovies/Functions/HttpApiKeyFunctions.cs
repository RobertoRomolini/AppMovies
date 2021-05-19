using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using AppMovies.Repository;
using AppMovies.Util;

namespace AppMovies.Functions
{
    public class HttpApiKeyFunctions
    {
        private readonly IApiKeyCrudRepository _apiKeyRepository;

        public HttpApiKeyFunctions(IApiKeyCrudRepository apiKeyRepository)
        {
            _apiKeyRepository = apiKeyRepository;
        }

        [FunctionName("GetNewKey")]
        public async Task<IActionResult> GetNewKey(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "update-key")] HttpRequest req,
            ILogger log)
        {

            if (req.Headers["x-api-key"] == "password")
            {
                var key = new byte[32];
                var generator = RandomNumberGenerator.Create();
                generator.GetBytes(key);
                string apiKey = Convert.ToBase64String(key);

                string encryptedApiKey = StaticMethods.Hash(apiKey);

                await _apiKeyRepository.Update(encryptedApiKey);
                return new OkObjectResult(apiKey);
            }
            else
            {
                var result = new ObjectResult("Invalid password");
                result.StatusCode = StatusCodes.Status401Unauthorized;
                return result;
            }
        }
    }
}
