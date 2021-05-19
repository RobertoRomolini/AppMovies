using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppMovies.Models;
using AppMovies.Repository;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using static AppMovies.Startup;

namespace AppMovies.Functions
{
    public class TimerFunctions
    {
        private readonly IBlobCrudRepository _csvMovieCrudRepository;

        public TimerFunctions(ServiceBlobResolver serviceAccessor)
        {
            _csvMovieCrudRepository = serviceAccessor("CsvMovies");
        }

        // Run the function every first day of the month at 3:00 AM
        [FunctionName("CheckOldCsv")]
        public async Task CheckOldCsv([TimerTrigger("0 0 3 1 * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var blobList = await _csvMovieCrudRepository.GetFilesList();

            foreach (GenericBlob genericBlob in blobList)
            {
                // Check if the file is older than 30 days
                if (DateTimeOffset.Compare(genericBlob.LastModified .AddDays(30), DateTimeOffset.UtcNow) <= 0)
                {
                    await _csvMovieCrudRepository.Delete(genericBlob.Name);
                }
            }

        }
    }
}
