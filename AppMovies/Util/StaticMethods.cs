using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace AppMovies.Util
{
    public static class StaticMethods
    {
        public static string Hash (string stringToHash)
        {
            SHA256 sha256Hash = SHA256.Create();

            // ComputeHash - returns byte array  
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(stringToHash));

            // Convert byte array to a string   
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

        public static byte[] GetImageFromUrl(string url)
        {
            WebClient webClient = new WebClient();
            byte[] imageBytes = webClient.DownloadData(url);
            return imageBytes;
        }
    }
}
