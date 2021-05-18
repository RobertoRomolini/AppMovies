using AppMovies.Providers;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppMovies.Repository
{
    public class MovieImageCrudRepository : IBlobCrudRepository
    {
        private readonly CloudBlobContainer _container;
        public MovieImageCrudRepository(IStorageServiceProvider storageTableProvider)
        {
            _container = storageTableProvider.GetBlobContainer("movieimages");
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
