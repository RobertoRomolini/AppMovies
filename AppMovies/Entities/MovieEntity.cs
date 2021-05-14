using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMovies.Entities
{
    public class MovieEntity : TableEntity
    {
        public MovieEntity(string id)
        {
            this.PartitionKey = "movie";
            this.RowKey = id;
        }

        public string Id { get { return this.RowKey; } }
        public string Title { set; get; }
        public string Director { set; get; }
        public string Genre { set; get; }
        public int Year { set; get; }
        public string Country { set; get; }
        public string ImageUrl { set; get; }
    }
}
