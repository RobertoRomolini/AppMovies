using AppMovies.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppMovies.Repository
{
    public interface IBlobCrudRepository
    {
        Task<List<GenericBlob>> GetFilesList();
        //Task<MemoryStream> Get(string blobName);
        Task Add(byte[] file, string blobName);
        //Task Update(byte[] file , string blobName);
        Task Delete(string blobName);
        Task DeleteFolder(string folderName);
    }
}
