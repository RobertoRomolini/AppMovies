using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AppMovies.Entities;
using AppMovies.Repository;
using System.Web;
using System.Linq;
using AppMovies.Models;
using AppMovies.Util;

namespace AppMovies.Functions
{
    public class HttpMovieFunctions
    {
        private readonly ITableCrudRepository<MovieEntity> _repository;
        private readonly IApiKeyCrudRepository _apiKeyRepository;
        private readonly IBlobCrudRepository _blobCrudRepository;
        public HttpMovieFunctions(ITableCrudRepository<MovieEntity> repository , IApiKeyCrudRepository apiKeyRepository ,
            IBlobCrudRepository blobCrudRepository)
        {
            _repository = repository;
            _apiKeyRepository = apiKeyRepository;
            _blobCrudRepository = blobCrudRepository;
        }

        [FunctionName("GetMovies")]
        public async Task<IActionResult> GetMovies(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "movies")] HttpRequest req,
            ILogger log)
        {
            var collection = HttpUtility.ParseQueryString(req.QueryString.ToString());
            string parameters = JsonConvert.SerializeObject(collection.AllKeys.ToDictionary(y => y, y => collection[y]));
            return new OkObjectResult(await _repository.GetAll(req.QueryString));
        }


        [FunctionName("GetMovie")]
        public async Task<IActionResult> GetMovie(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "movies/{id}")] HttpRequest req,
            ILogger log , string id)
        {
            return new OkObjectResult(await _repository.Get(id));
        }

        [FunctionName("AddMovie")]
        public async Task<IActionResult> AddMovie(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "movies")] HttpRequest req,
            ILogger log)
        {
            try
            {
                string apiKey = req.Headers["x-api-key"];
                string encryptedApiKey = StaticMethods.Encrypt(apiKey);
                if (encryptedApiKey == await _apiKeyRepository.Get())
                {
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    Movie movie = JsonConvert.DeserializeObject<Movie>(requestBody);
                    MovieEntity movieEntity = new MovieEntity(movie);
                    await _repository.Add(movieEntity);
                    return new OkObjectResult(movieEntity);
                }
                else
                {
                    var result = new ObjectResult("Api Key Invalid");
                    result.StatusCode = StatusCodes.Status401Unauthorized;
                    return result;
                }
            }
            catch (Exception exception)
            {
                log.LogError(exception.ToString());
                var result = new ObjectResult(exception.Message);
                result.StatusCode = StatusCodes.Status500InternalServerError;
                return result;
            }
        }

        [FunctionName("UpdateMovie")]
        public async Task<IActionResult> UpdateMovie(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "movies/{id}")] HttpRequest req,
            ILogger log , string id)
        {
            try
            {
                string apiKey = req.Headers["x-api-key"];
                if (apiKey == await _apiKeyRepository.Get())
                {
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    Movie movie = JsonConvert.DeserializeObject<Movie>(requestBody);
                    MovieEntity movieEntity = new MovieEntity(movie)
                    {
                        RowKey = id,
                        ETag = "*"
                    };
                    await _repository.Update(movieEntity);
                    return new OkObjectResult(movieEntity);
                }
                else
                {
                    var result = new ObjectResult("Api Key Invalid");
                    result.StatusCode = StatusCodes.Status401Unauthorized;
                    return result;
                }
            }
            catch (Exception exception)
            {
                log.LogError(exception.ToString());
                var result = new ObjectResult(exception.Message);
                result.StatusCode = StatusCodes.Status500InternalServerError;
                return result;
            }
        }

        [FunctionName("DeleteMovie")]
        public async Task<IActionResult> DeleteMovie(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "movies/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            try
            {
                string apiKey = req.Headers["x-api-key"];
                if (apiKey == await _apiKeyRepository.Get())
                {
                    await _repository.Delete(id);
                    await _blobCrudRepository.Delete(id + ".jpg");
                    return new NoContentResult();
                }
                else
                {
                    var result = new ObjectResult("Api Key Invalid");
                    result.StatusCode = StatusCodes.Status401Unauthorized;
                    return result;
                }
            }
            catch (Exception exception)
            {
                log.LogError(exception.ToString());
                var result = new ObjectResult(exception.Message);
                result.StatusCode = StatusCodes.Status500InternalServerError;
                return result;
            }
        }
    }
}
