using Kozachok.Repository.Config;
using Kozachok.Shared.Abstractions.Mapping;
using Kozachok.Shared.DTO.Models.DbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Kozachok.Repository.Mapping
{
    internal class PlaylistMovieMap : DbEntityConfiguration<PlaylistMovie>, IEntityMap
    {
        public override void Configure(EntityTypeBuilder<PlaylistMovie> entity)
        {
            entity.ToTable("T_Playlist_Movie", "dbo");
            entity.Property(p => p.Id).HasColumnName("Id");
            entity.Property(p => p.Id).ValueGeneratedOnAdd();
        }
    }
}
