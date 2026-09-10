using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.DAL.Entities
{
    public class PlaylistEntity
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public Guid CreatorId { get; set; }

        public UserEntity Creator { get; set; }

        public string? CoverImageKey { get; set; }

        public bool IsPrivate { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<PlaylistTrackEntity> PlaylistTracks { get; set; } = new List<PlaylistTrackEntity>();

        public ICollection<PlaylistLikeEntity> Likes { get; set; } = new List<PlaylistLikeEntity>();
    }
}
