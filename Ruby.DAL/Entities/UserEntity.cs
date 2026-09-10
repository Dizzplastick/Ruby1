using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.DAL.Entities
{
    public class UserEntity
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string? Email { get; set; }

        public string HashPassword { get; set; }

        public string? AvatarKey { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation prop

        public ICollection<RefreshTokenEntity> RefreshTokens { get; set; } = new List<RefreshTokenEntity>();

        public ICollection<TrackEntity> Tracks { get; set; } = new List<TrackEntity>();

        public ICollection<PlaylistEntity> Playlists { get; set; } = new List<PlaylistEntity>();

        public ICollection<LikeEntity> Likes { get; set; } = new List<LikeEntity>();

        public ICollection<PlaylistLikeEntity> LikedPlaylists { get; set; } = new List<PlaylistLikeEntity>();







    }
}
