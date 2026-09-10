using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.DAL.Entities
{
    public class TrackGenreEntity
    {
        public Guid TrackId { get; set; }

        public TrackEntity Track { get; set; }

        public int GenreId { get; set; }

        public GenreEntity Genre { get; set; }
    }
}
