using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AppMovies.Util
{
    public class ImagesDownloader : IImagesDownloader
    {
        public byte[] GetImageFromUrl(string url)
        {
            WebClient webClient = new WebClient();
            byte[] imageBytes = webClient.DownloadData(url);
            return imageBytes;
        }
    }
}
