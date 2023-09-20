using Kozachok.Repository.Config;
using Kozachok.Shared.Abstractions.Mapping;
using Kozachok.Shared.DTO.Models.DbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kozachok.Repository.Mapping
{
    internal class MovieMap : DbEntityConfiguration<Movie>, IEntityMap
    {
        public override void Configure(EntityTypeBuilder<Movie> entity)
        {
            entity.ToTable("T_Movie", "dbo");
            entity.Property(p => p.Id).HasColumnName("Id");
            entity.Property(p => p.Id).ValueGeneratedOnAdd();


            entity.HasMany(x => x.PlaylistMovies)
                .WithOne(x => x.Movie);
        }
    }
}
