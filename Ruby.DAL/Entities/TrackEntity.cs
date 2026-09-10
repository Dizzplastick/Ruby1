namespace Ruby.DAL.Entities
{
    public class TrackEntity
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public Guid AuthorId { get; set; }

        public UserEntity Author { get; set; }

        public string AudioFileKey { get; set; }

        public string? CoverImageKey { get; set; }

        public int DurationSeconds { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation prop
        

        public ICollection<LikeEntity> Likes { get; set; } = new List<LikeEntity>();

        public ICollection<PlaylistTrackEntity> PlaylistTracks { get; set; } = new List<PlaylistTrackEntity>();

        public ICollection<TrackGenreEntity> TrackGenres { get; set; } = new List<TrackGenreEntity>();
    }
}
