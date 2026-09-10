using Microsoft.EntityFrameworkCore;
using Ruby.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Ruby.DAL
{
    public class RubyDBContext : DbContext
    {
        public RubyDBContext(DbContextOptions<RubyDBContext> options) : base(options)
        {
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<TrackEntity> Tracks { get; set; }
        public DbSet<PlaylistEntity> Playlists { get; set; }
        public DbSet<PlaylistTrackEntity> PlaylistTracks { get; set; }
        public DbSet<LikeEntity> Likes { get; set; }
        public DbSet<GenreEntity> Genres { get; set; }
        public DbSet<TrackGenreEntity> TrackGenres { get; set; }
        public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
        public DbSet<PlaylistLikeEntity> PlaylistLikes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
