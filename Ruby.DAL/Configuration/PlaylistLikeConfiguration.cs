using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ruby.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.DAL.Configuration
{
    public class PlaylistLikeConfiguration : IEntityTypeConfiguration<PlaylistLikeEntity>
    {
        public void Configure(EntityTypeBuilder<PlaylistLikeEntity> builder)
        {
            builder.ToTable("playlist_likes");

            builder.HasKey(pl => new { pl.UserId, pl.PlaylistId });
            builder.Property(pl => pl.UserId)
                .HasColumnName("user_id")
                .IsRequired();
            builder.Property(pl => pl.PlaylistId)
                .HasColumnName("playlist_id")
                .IsRequired();

            builder.Property(pl => pl.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.HasOne(pl => pl.User)
                .WithMany(u => u.LikedPlaylists)
                .HasForeignKey(pl => pl.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pl => pl.Playlist)
                .WithMany(p => p.Likes)
                .HasForeignKey(pl => pl.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
