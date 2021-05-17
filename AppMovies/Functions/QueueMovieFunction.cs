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

namespace AppMovies.Functions
{
    public class QueueMovieFunction
    {
        private readonly ITableCrudRepository<MovieEntity> _movieEntityCrudRepository;
        private readonly IBlobCrudRepository _blobCrudRepository;
        private readonly IImagesDownloader _imagesDownloader;

        public QueueMovieFunction(ITableCrudRepository<MovieEntity> repository , IBlobCrudRepository blobCrudRepository,
            IImagesDownloader imagesDownloader)
        {
            _movieEntityCrudRepository = repository;
            _blobCrudRepository = blobCrudRepository;
            _imagesDownloader = imagesDownloader;
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
                    var imageBytes = _imagesDownloader.GetImageFromUrl(movieEntity.ImageUrl);

                    //Set filename equal to the storage table id
                    string fileName = movieEntity.Id + Path.GetExtension(movieEntity.ImageUrl);

                    //Add or update entity to the storage table movies
                    await _movieEntityCrudRepository.AddOrUpdate(movieEntity);

                    //Add image to the storage blob imagemovies
                    await _blobCrudRepository.Add(imageBytes, fileName);
                }
                else
                {
                    //Delete the entity from the storage table movies
                    await _movieEntityCrudRepository.Delete(movieEntity.Id);

                    await _blobCrudRepository.Delete(movieEntity.Id + ".jpg");
                }
                
            }
            catch (Exception exception)
            {
                log.LogError(exception.Message);
            }
        }


    }
}
