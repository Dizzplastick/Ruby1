using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ruby.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ruby.Shared.Enums;

namespace Ruby.DAL.Configuration
{
    internal class GenreConfiguration : IEntityTypeConfiguration<Entities.GenreEntity>
    {
        public void Configure(EntityTypeBuilder<GenreEntity> builder)
        {
            builder.ToTable("genres");

            builder.HasKey(g => g.Id);
            builder.Property(g => g.Id)
                .HasColumnName("id")
                .ValueGeneratedNever()
                .IsRequired(true);

            builder.Property(g => g.Name)
                .HasColumnName("name")
                .IsRequired(true)
                .HasMaxLength(255);

            var genres = Enum.GetValues(typeof(TrackGenresEnum))
                         .Cast<TrackGenresEnum>()
                         .Select(g => new GenreEntity
                         {
                             Id = (int)g,
                             Name = g.ToString() 
                         });

            builder.HasData(genres);


        }
    }
}
