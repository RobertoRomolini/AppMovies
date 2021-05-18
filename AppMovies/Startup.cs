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
            builder.Services.AddSingleton<IStorageServiceProvider, StorageServiceProvider>();
            builder.Services.AddSingleton<ICsvMovieConverter, CsvMovieConverter>();

            builder.Services.AddSingleton<ITableCrudRepository<MovieEntity>, MovieCrudRepository>();
            builder.Services.AddSingleton<IApiKeyCrudRepository, ApiKeyCrudRepository>();
            builder.Services.AddSingleton<IBlobCrudRepository, MovieImageCrudRepository>();
            builder.Services.AddSingleton<ICsvSplittedCrudRepository, CsvSplittedCrudRepository>();
            builder.Services.AddLogging();
            
        }
    }
}