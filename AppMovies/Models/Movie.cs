using System;
using System.Collections.Generic;
using System.Text;

namespace AppMovies.Models
{
    public class Movie
    {
        public string Id { set; get; }
        public string Title { set; get; }
        public string Director { set; get; }
        public string Genre { set; get; }
        public string Year { set; get; }
        public string Country { set; get; }
        public string ImageUrl { set; get; }
    }
}
