using AppMovies.Providers;
using AppMovies.Util;
using AppMovies.Repository;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using AppMovies.Entities;

[assembly: FunctionsStartup(typeof(AppMovies.Startup))]
namespace AppMovies
{
    public class Startup : FunctionsStartup
    {

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IStorageTableProvider, StorageTableProvider>();
            builder.Services.AddSingleton<IStorageQueueProvider, StorageQueueProvider>();
            builder.Services.AddSingleton<IStorageBlobProvider, StorageBlobProvider>();
            builder.Services.AddSingleton<ICsvMovieConverter, CsvMovieConverter>();
            builder.Services.AddSingleton<IImagesDownloader, ImagesDownloader>();

            builder.Services.AddSingleton<ITableCrudRepository<MovieEntity>, MovieCrudRepository>();
            builder.Services.AddSingleton<IBlobCrudRepository, MovieImageCrudRepository>();
            builder.Services.AddLogging();
            
        }
    }
}