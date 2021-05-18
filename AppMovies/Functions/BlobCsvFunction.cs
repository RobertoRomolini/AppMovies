using System.Collections.Generic;
using System.IO;
using AppMovies.Providers;
using AppMovies.Repository;
using AppMovies.Models;
using AppMovies.Util;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AppMovies
{
    public class BlobCsvFunction
    {
        private readonly ICsvMovieConverter _csvMovieConverter;
        private readonly ICsvSplittedCrudRepository _csvSplittedCrudRepository;
        private readonly CloudQueue _queue;

        public BlobCsvFunction(ICsvMovieConverter csvMovieConverter , IStorageServiceProvider storageServiceProvider , ICsvSplittedCrudRepository csvSplittedCrudRepository)
        {
            _csvMovieConverter = csvMovieConverter;
            _queue = storageServiceProvider.GetQueue("movies");
            _csvSplittedCrudRepository = csvSplittedCrudRepository;
        }

        [FunctionName("CsvFunction")]
        public async Task CsvFunction([BlobTrigger("csvmovies/{name}", Connection = "AzureWebJobsStorage")]Stream csvBlob, string name, ILogger log)
        {
            int rowLimit = 5000;
            
            if (_csvMovieConverter.CsvRowCount(csvBlob) > rowLimit)
            {
                _csvMovieConverter.SplitCsv(csvBlob , rowLimit);
            }
            else
            {
                await AddMessageQueue(csvBlob , name);
            }
        }

        [FunctionName("BlobCsvSplittedFunction")]
        public async Task BlobCsvSplittedFunction([BlobTrigger("csvsplitted/{name}", Connection = "AzureWebJobsStorage")] Stream csvBlob, string name, ILogger log)
        {
            await AddMessageQueue(csvBlob , name);
            await _csvSplittedCrudRepository.Delete(name);
        }

        private async Task AddMessageQueue (Stream csvBlob , string blobName)
        {
            List<Movie> movies = _csvMovieConverter.GetMoviesFromCsv(csvBlob);

            foreach (Movie movie in movies)
            {
                string jsonMovie = JsonSerializer.Serialize(movie);
                CloudQueueMessage message = new CloudQueueMessage(jsonMovie);
                await _queue.AddMessageAsync(message);
            }
        }
    }
}
