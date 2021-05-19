using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AppMovies.Entities;
using AppMovies.Models;
using AppMovies.Repository;
using AppMovies.Util;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static AppMovies.Startup;

namespace AppMovies.Functions
{
    public class QueueMovieFunction
    {
        private readonly ITableCrudRepository<MovieEntity> _movieEntityCrudRepository;
        private readonly IBlobCrudRepository _movieImageCrudRepository;

        public QueueMovieFunction(ITableCrudRepository<MovieEntity> movieEntityCrudRepository, ServiceBlobResolver serviceAccessor)
        {
            _movieEntityCrudRepository = movieEntityCrudRepository;
            _movieImageCrudRepository = serviceAccessor("MovieImage");
        }

        [FunctionName("QueueMovieFunction")]
        public async Task Run([QueueTrigger("movies", Connection = "AzureWebJobsStorage")]string myQueueItem, ILogger log)
        {
            try
            {
                Movie movie = JsonConvert.DeserializeObject<Movie>(myQueueItem);

                MovieEntity movieEntity = new MovieEntity(movie);

                if (movieEntity.Title != "")
                {
                    //Download the image from the Url
                    var imageBytes = StaticMethods.GetImageFromUrl(movieEntity.ImageUrl);

                    //Add or update entity to the storage table movies
                    await _movieEntityCrudRepository.AddOrUpdate(movieEntity);

                    //Add image to the storage blob imagemovies
                    await _movieImageCrudRepository.Add(imageBytes, movieEntity.Id);
                }
                else
                {
                    //Delete the entity from the storage table movies
                    await _movieEntityCrudRepository.Delete(movieEntity.Id);
                    //Delete the image from the storage blob movieimages
                    await _movieImageCrudRepository.Delete(movieEntity.Id);
                }
            }
            catch (Exception exception)
            {
                log.LogError(exception.Message);
            }
        }


    }
}
