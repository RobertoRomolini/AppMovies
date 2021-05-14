using System;
using System.Collections.Generic;
using System.IO;
using AppMovies.Entities;
using AppMovies.Providers;
using AppMovies.Models;
using AppMovies.Util;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
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

        [FunctionName("BlobCsvFunction")]
        public async Task Run([BlobTrigger("csvmovies/{name}", Connection = "AzureWebJobsStorage")]Stream csvBlob, string name, ILogger log)
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
