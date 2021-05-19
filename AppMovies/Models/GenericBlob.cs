using System;
using System.Collections.Generic;
using System.Text;

namespace AppMovies.Models
{
    public class GenericBlob
    {
        public string Name { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }

        public override string ToString()
        {
            return "Name: " + Name + ", Last Modified: " + LastModified + ", Content Type: " + ContentType + ", Size: " + Size/1000 + "Kb";
        }
    }
}
