using AppMovies.Models;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMovies.Entities
{
    public class MovieEntity : TableEntity
    {
        public MovieEntity()
        {
        }
        public MovieEntity (Movie movie)
        {
            this.PartitionKey = "movie";
            this.RowKey = movie.Id;
            this.Title = movie.Title;
            this.Director = movie.Director;
            this.Genre = movie.Genre;
            this.Year = movie.Year;
            this.Country = movie.Country;
            this.ImageUrl = movie.ImageUrl;
        }

        public string Id { get { return this.RowKey; } }
        public string Title { set; get; }
        public string Director { set; get; }
        public string Genre { set; get; }
        public string Year { set; get; }
        public string Country { set; get; }
        public string ImageUrl { set; get; }
    }
}
