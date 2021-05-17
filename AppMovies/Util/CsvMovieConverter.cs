using AppMovies.Entities;
using AppMovies.Models;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AppMovies.Util
{
    public class CsvMovieConverter : ICsvMovieConverter
    {
        public List<Movie> GetMoviesFromCsv (Stream csvBlob)
        {
            List<Movie> movies = new List<Movie>();

            var streamReader = new StreamReader(csvBlob);
            var csv = new CsvReader(streamReader , System.Globalization.CultureInfo.CurrentCulture);
                
            if (csv.Read())
            {   
                csv.ReadHeader();
                while (csv.Read())
                {
                    var movie = new Movie();
                    movie.Id = csv.GetField("id");
                    movie.Title = csv.GetField("title");
                    movie.Director = csv.GetField("director");
                    movie.Genre = csv.GetField("genre");
                    movie.Year = csv.GetField("year");
                    movie.Country = csv.GetField("country");
                    movie.ImageUrl = csv.GetField("imageurl");
                    movies.Add(movie);
                }
            }
            return movies;
        }
    }
}
