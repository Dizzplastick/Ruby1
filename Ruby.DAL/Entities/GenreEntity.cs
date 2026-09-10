using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.DAL.Entities
{
    public class GenreEntity 
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<TrackGenreEntity> TrackGenres { get; set; } = new List<TrackGenreEntity>();
    }
}
