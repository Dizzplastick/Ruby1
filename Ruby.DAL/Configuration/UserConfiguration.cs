using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ruby.DAL.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<Entities.UserEntity>
    {
        public void Configure(EntityTypeBuilder<Entities.UserEntity> builder)
        {
            builder.ToTable("users");


            //id PK
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()")
                .IsRequired(true);


            builder.Property(u => u.Username)
                .HasColumnName("username")
                .IsRequired(true)
                .HasMaxLength(100);
            builder.HasIndex(u => u.Username)
                .IsUnique();


            builder.Property(u => u.Email)
                .HasColumnName("email")
                .IsRequired(false)
                .HasMaxLength(150);
            builder.HasIndex(u => u.Email);
                



            builder.Property(u => u.HashPassword)
                .HasColumnName("hash_password")
                .IsRequired(true)
                .HasMaxLength(255);

            builder.Property(u => u.AvatarKey)
                .HasColumnName("avatar_url")
                .IsRequired(false)
                .HasMaxLength(1024);

            builder.Property(u => u.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired(true)
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            //relationssssssssssshit

            //2 tracks
            builder.HasMany(u => u.Tracks)
                .WithOne(t => t.Author)
                .HasForeignKey(t => t.AuthorId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);//cascade ne sosatt

            //2 playlists
            builder.HasMany(u => u.Playlists)
                .WithOne(p => p.Creator)
                .HasForeignKey(t => t.CreatorId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            //2 likke
            builder.HasMany(u => u.Likes)
               .WithOne(p => p.User)
               .HasForeignKey(t => t.UserId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);

            //2 refrsh tokens
            builder.HasMany(u => u.RefreshTokens)
               .WithOne(p => p.User)
               .HasForeignKey(t => t.UserId)
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
