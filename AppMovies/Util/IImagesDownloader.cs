using System;
using System.Collections.Generic;
using System.Text;

namespace AppMovies.Util
{
    public interface IImagesDownloader
    {
        byte[] GetImageFromUrl(string url);
    }
}
