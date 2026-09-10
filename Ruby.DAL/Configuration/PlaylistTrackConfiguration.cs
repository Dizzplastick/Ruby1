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
    public class PlaylistTrackConfiguration : IEntityTypeConfiguration<Entities.PlaylistTrackEntity>
    {
        public void Configure(EntityTypeBuilder<PlaylistTrackEntity> builder)
        {
            builder.ToTable("playlist_tracks");

            builder.HasKey(pt => new { pt.PlaylistId, pt.TrackId });
            builder.Property(pt => pt.TrackId)
                .HasColumnName("track_id")
                .IsRequired(true);
            builder.Property(pt => pt.PlaylistId)
               .HasColumnName("playlist_id")
               .IsRequired(true);


            builder.Property(pt => pt.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired(true);
        }
    }
}
