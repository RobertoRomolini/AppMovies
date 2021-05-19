using AppMovies.Providers;
using AppMovies.Util;
using AppMovies.Repository;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using AppMovies.Entities;
using System.Collections.Generic;

[assembly: FunctionsStartup(typeof(AppMovies.Startup))]
namespace AppMovies
{
    public class Startup : FunctionsStartup
    {
        public delegate IBlobCrudRepository ServiceBlobResolver(string key);
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();

            builder.Services.AddSingleton<IStorageServiceProvider, StorageServiceProvider>();
            builder.Services.AddSingleton<ICsvMovieConverter, CsvMovieConverter>();

            builder.Services.AddSingleton<ITableCrudRepository<MovieEntity>, MovieCrudRepository>();
            builder.Services.AddSingleton<IApiKeyCrudRepository, ApiKeyCrudRepository>();
            
            //--------------------------------------------------------------------------------------------------//
            //                                  IBlobCrudRepository
            //--------------------------------------------------------------------------------------------------//
            builder.Services.AddSingleton<MovieImageCrudRepository>();
            builder.Services.AddSingleton<CsvSplittedCrudRepository>();
            builder.Services.AddSingleton<ServiceBlobResolver>(serviceProvider => key =>
            {
                switch (key)
                {
                    case "MovieImage":
                        return serviceProvider.GetService<MovieImageCrudRepository>();
                    case "CsvSplitted":
                        return serviceProvider.GetService<CsvSplittedCrudRepository>();
                    default:
                        throw new KeyNotFoundException();
                }
            });

        }
    }
}