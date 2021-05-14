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
        public List<MovieEntity> GetMoviesFromCsv (Stream csvBlob)
        {
            List<MovieEntity> movies = new List<MovieEntity>();

            var streamReader = new StreamReader(csvBlob);
            var csv = new CsvReader(streamReader , System.Globalization.CultureInfo.CurrentCulture);
                
            if (csv.Read())
            {   
                csv.ReadHeader();
                while (csv.Read())
                {
                    var movieEntity = new MovieEntity(csv.GetField("id"));
                    movieEntity.Title = csv.GetField("title");
                    movieEntity.Director = csv.GetField("director");
                    movieEntity.Genre = csv.GetField("genre");
                    movieEntity.Year = Int32.Parse( csv.GetField("year"));
                    movieEntity.Country = csv.GetField("country");
                    movieEntity.ImageUrl = csv.GetField("imageurl");
                    movies.Add(movieEntity);
                }
            }

            return movies;
        }
    }
}
