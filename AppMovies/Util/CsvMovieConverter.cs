using AppMovies.Entities;
using AppMovies.Models;
using AppMovies.Providers;
using AppMovies.Repository;
using CsvHelper;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static AppMovies.Startup;

namespace AppMovies.Util
{
    public class CsvMovieConverter : ICsvMovieConverter
    {
        private readonly IBlobCrudRepository _csvSplittedCrudRepository;
        public CsvMovieConverter(ServiceBlobResolver serviceAccessor)
        {
            _csvSplittedCrudRepository = serviceAccessor("CsvSplitted");
        }

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
            csvBlob.Position = 0;
            return movies;
        }

        public void SplitCsv(Stream csvBlob , int rowNumbers)
        {
            var streamReader = new StreamReader(csvBlob);
            
            string header = streamReader.ReadLine();
            while (!streamReader.EndOfStream)
            {
                int count = 0;
                string fileName = Guid.NewGuid().ToString();

                MemoryStream outStream = new MemoryStream();
                StreamWriter streamWriter = new StreamWriter(outStream);
                streamWriter.WriteLine(header);
                streamWriter.AutoFlush = true;

                while (!streamReader.EndOfStream && ++count < rowNumbers)
                {
                    streamWriter.WriteLine(streamReader.ReadLine());
                }

                _csvSplittedCrudRepository.Add(outStream.ToArray(), fileName);
            }
            //Set the position to read again the file
            csvBlob.Position = 0;
        }

        public int CsvRowCount (Stream csvBlob)
        {
            var streamReader = new StreamReader(csvBlob);
            int rowCount = 0;
            while (!streamReader.EndOfStream)
            {
                streamReader.ReadLine();
                rowCount++;
            }
            csvBlob.Position = 0;
            return rowCount;
        }
    }
}
