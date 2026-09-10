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
    public class PlaylistConfiguration : IEntityTypeConfiguration<Entities.PlaylistEntity>
    {
        public void Configure(EntityTypeBuilder<PlaylistEntity> builder)
        {
            builder.ToTable("playlists");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()")
                .IsRequired(true);

           builder.Property(p => p.Title)
                .HasColumnName("title")
                .HasColumnType("varchar(200)")
                .IsRequired(true);
            builder.HasIndex(p => p.Title);

            builder.Property(p => p.CreatorId)
                .HasColumnName("creator_id")
                .IsRequired(true);

            

            builder.Property(p => p.CoverImageKey)
                .HasColumnName("cover_image_url")
                .HasColumnType("varchar(1024)")
                .IsRequired(false);

            builder.Property(p => p.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired(true);

            builder.Property(p => p.IsPrivate)
                .HasColumnName("is_private")
                .HasDefaultValue(false) 
                .HasColumnType("boolean")
                .IsRequired();

            //relationships

            //to playlist_Tracks
            builder.HasMany(p => p.PlaylistTracks)
                .WithOne(pt => pt.Playlist)
                .HasForeignKey(pt => pt.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
