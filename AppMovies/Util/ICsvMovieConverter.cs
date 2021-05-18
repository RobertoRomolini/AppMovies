using AppMovies.Entities;
using AppMovies.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AppMovies.Util
{
    public interface ICsvMovieConverter
    {
        List<Movie> GetMoviesFromCsv(Stream csvBlob);
        void SplitCsv(Stream csvBlob , int rowNumbers);
        int CsvRowCount(Stream csvBlob);
    }
}
