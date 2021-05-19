using AppMovies.Providers;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppMovies.Repository
{
    public class CsvSplittedCrudRepository : IBlobCrudRepository
    {
        private readonly CloudBlobContainer _container;
        public CsvSplittedCrudRepository(IStorageServiceProvider storageTableProvider)
        {
            _container = storageTableProvider.GetBlobContainer("csvsplitted");
        }

        public async Task Add(byte[] file , string blobName)
        {   
            CloudBlockBlob blob = _container.GetBlockBlobReference(blobName);
            await blob.UploadFromByteArrayAsync(file, 0, file.Length);
        }

        public async Task Delete(string blobName)
        {
            CloudBlockBlob blob = _container.GetBlockBlobReference(blobName);
            await blob.DeleteAsync();
        }
    }
}
