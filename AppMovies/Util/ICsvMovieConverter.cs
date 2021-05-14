using AppMovies.Entities;
using AppMovies.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AppMovies.Util
{
    public interface ICsvMovieConverter
    {
        List<MovieEntity> GetMoviesFromCsv(Stream csvBlob);
    }
}
