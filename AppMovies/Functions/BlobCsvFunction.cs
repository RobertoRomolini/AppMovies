using System.Collections.Generic;
using System.IO;
using AppMovies.Providers;
using AppMovies.Models;
using AppMovies.Util;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Threading.Tasks;

namespace AppMovies
{
    public class BlobCsvFunction
    {
        private readonly ICsvMovieConverter _csvMovieConverter;
        private readonly CloudQueue _queue;

        public BlobCsvFunction(ICsvMovieConverter csvMovieConverter , IStorageQueueProvider queue)
        {
            _csvMovieConverter = csvMovieConverter;
            _queue = queue.GetQueue("movies");
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
                await AddMessageQueue(csvBlob);
            }

        }

        [FunctionName("BlobCsvSplittedFunction")]
        public async Task BlobCsvSplittedFunction([BlobTrigger("csvsplitted/{name}", Connection = "AzureWebJobsStorage")] Stream csvBlob, string name, ILogger log)
        {
            await AddMessageQueue(csvBlob);
        }

        private async Task AddMessageQueue (Stream csvBlob)
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
