using AppMovies.Models;
using AppMovies.Providers;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;

namespace AppMovies.Repository
{
    class CsvMoviesCrudRepository : IBlobCrudRepository
    {
        private readonly BlobContainerClient _blobContainerClient;
        private readonly CloudBlobContainer _blobContainer;
        public CsvMoviesCrudRepository(IStorageServiceProvider storageTableProvider)
        {
            _blobContainerClient = storageTableProvider.GetContainerClient("csvmovies");
            _blobContainer = storageTableProvider.GetBlobContainer("csvmovies");
        }

        public async Task<List<GenericBlob>> GetFilesList()
        {
            List<GenericBlob> blobList = new List<GenericBlob>();

            await foreach (BlobItem blobItem in _blobContainerClient.GetBlobsAsync())
            {
                GenericBlob genericBlob = new GenericBlob();
                genericBlob.Name = blobItem.Name;
                genericBlob.LastModified = (DateTimeOffset) blobItem.Properties.LastModified;
                genericBlob.ContentType = blobItem.Properties.ContentType;
                genericBlob.Size = (long) blobItem.Properties.ContentLength;
                blobList.Add(genericBlob);
            }
            return blobList;
        }

        public async Task Add(byte[] file, string blobName)
        {
            CloudBlockBlob blob = _blobContainer.GetBlockBlobReference(blobName);
            await blob.UploadFromByteArrayAsync(file, 0, file.Length);
        }

        public async Task Delete(string blobName)
        {
            CloudBlockBlob blob = _blobContainer.GetBlockBlobReference(blobName);
            await blob.DeleteAsync();
        }
        public async Task DeleteFolder(string folderName)
        {
            var blobDirectory = _blobContainer.GetDirectoryReference(folderName);

            var blobs = await blobDirectory.ListBlobsSegmentedAsync(false, BlobListingDetails.Metadata, 100, null, null, null);

            foreach (IListBlobItem blob in blobs.Results)
            {
                if (blob.GetType() == typeof(CloudBlob) || blob.GetType().BaseType == typeof(CloudBlob))
                {
                    await ((CloudBlob)blob).DeleteIfExistsAsync();
                }
            }
        }
    }
}
