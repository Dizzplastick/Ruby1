using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.DAL.Configuration
{
    public class RefreshTokensConfiguration : IEntityTypeConfiguration<Entities.RefreshTokenEntity>
    {
        public void Configure(EntityTypeBuilder<Entities.RefreshTokenEntity> builder) {

            builder.ToTable("refresh_tokens");

            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()")
                .IsRequired();

            builder.Property(r => r.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(r => r.RefreshToken)
                .HasColumnName("refresh_token")
                .HasColumnType("varchar(128)")
                .IsRequired();

            builder.HasIndex(r => r.RefreshToken)
                .IsUnique();

            builder.Property(r => r.ExpiresAt)
                .HasColumnName("expires_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.HasIndex(r => r.ExpiresAt);

            builder.Property(r => r.CreatedAt)
                .HasColumnName("creation_at")
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

        }
    }
}
