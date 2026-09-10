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
    public class TrackGenreConfiguration : IEntityTypeConfiguration<TrackGenreEntity>
    {
        public void Configure(EntityTypeBuilder<TrackGenreEntity> builder)
        {
            builder.ToTable("track_genres");

            builder.HasKey(tg => new { tg.TrackId, tg.GenreId });

            builder.Property(tg => tg.TrackId)
                   .HasColumnName("track_id")
                   .IsRequired();

            builder.Property(tg => tg.GenreId)
                    .HasColumnName("genre_id")
                    .IsRequired();

            // с треком
            builder.HasOne(tg => tg.Track)
                   .WithMany(t => t.TrackGenres)
                   .HasForeignKey(tg => tg.TrackId);

            //c жанром
            builder.HasOne(tg => tg.Genre)
                   .WithMany(g => g.TrackGenres)
                   .HasForeignKey(tg => tg.GenreId);
        }
    }
}
