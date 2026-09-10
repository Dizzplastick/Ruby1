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
    public class TrackConfiguration : IEntityTypeConfiguration<Entities.TrackEntity>
    {
        public void Configure(EntityTypeBuilder<TrackEntity> builder)
        {
            builder.ToTable("tracks");

            //id
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()")
                .IsRequired(true);

            builder.Property(t => t.Title)
                .HasColumnName("title")
                .HasColumnType("varchar(200)")
                .IsRequired(true);
            builder.HasIndex(t => t.Title);

            builder.Property(t => t.AuthorId)
                .HasColumnName("author_id")
                .IsRequired(true);

            builder.Property(t => t.AudioFileKey)
                .HasColumnName("audio_file_key")
                .HasColumnType("varchar")
                .IsRequired(true);

            builder.Property(t => t.CoverImageKey)
                .HasColumnName("cover_image_url")
                .HasColumnType("varchar(1024)")
                .IsRequired(false);

            builder.Property(t => t.DurationSeconds)
                .HasColumnName("duration_seconds")
                .IsRequired(true);

            builder.Property(t => t.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired(true);
            builder.HasIndex(t => t.CreatedAt);

            //relation with PlaylistTrackEntity
            builder.HasMany(t => t.PlaylistTracks)
                .WithOne(pt => pt.Track)
                .HasForeignKey(pt => pt.TrackId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            //relation with LikeEntity
            builder.HasMany(t => t.Likes)
                .WithOne(l => l.Track)
                .HasForeignKey(l => l.TrackId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
