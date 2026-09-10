using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.DAL.Entities
{
    public class PlaylistLikeEntity
    {
        public Guid UserId { get; set; }
        public UserEntity User { get; set; }

        public Guid PlaylistId { get; set; }
        public PlaylistEntity Playlist { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
