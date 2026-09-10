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
     public class LikeConfiguration : IEntityTypeConfiguration<Entities.LikeEntity>
    {
        public void Configure(EntityTypeBuilder<LikeEntity> builder)
        {
            builder.ToTable("likes");

            builder.HasKey(l => new { l.UserId, l.TrackId });
            builder.Property(l => l.UserId)
                .HasColumnName("user_id").
                IsRequired(true);
            builder.Property(l => l.TrackId)
                .HasColumnName("track_id").
                IsRequired(true);

            builder.Property(l => l.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired(true);

                


        }
    }
}
