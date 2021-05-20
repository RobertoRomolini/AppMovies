using AppMovies.Models;
using AppMovies.Providers;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppMovies.Repository
{
    public class MovieImageCrudRepository : IBlobCrudRepository
    {
        private readonly BlobContainerClient _blobContainerClient;
        private readonly CloudBlobContainer _blobContainer;
        public MovieImageCrudRepository(IStorageServiceProvider storageTableProvider)
        {
            _blobContainerClient = storageTableProvider.GetContainerClient("movieimages");
            _blobContainer = storageTableProvider.GetBlobContainer("movieimages");
        }

        public async Task<List<GenericBlob>> GetFilesList()
        {
            List<GenericBlob> blobList = new List<GenericBlob>();

            await foreach (BlobItem blobItem in _blobContainerClient.GetBlobsAsync())
            {
                GenericBlob genericBlob = new GenericBlob();
                genericBlob.Name = blobItem.Name;
                genericBlob.LastModified = (DateTimeOffset)blobItem.Properties.LastModified;
                genericBlob.ContentType = blobItem.Properties.ContentType;
                genericBlob.Size = (long)blobItem.Properties.ContentLength;
                blobList.Add(genericBlob);
            }
            return blobList;
        }

        public async Task Add(byte[] file , string blobName)
        {   
            CloudBlockBlob blob = _blobContainer.GetBlockBlobReference(blobName);
            await blob.UploadFromByteArrayAsync(file, 0, file.Length);
        }

        public async Task Delete(string blobName)
        {
            CloudBlockBlob blob = _blobContainer.GetBlockBlobReference(blobName);
            await blob.DeleteAsync();
        }
    }
}
