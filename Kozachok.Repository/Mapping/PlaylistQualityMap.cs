using Kozachok.Repository.Config;
using Kozachok.Shared.Abstractions.Mapping;
using Kozachok.Shared.DTO.Models.DbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kozachok.Repository.Mapping
{
    internal class PlaylistQualityMap : DbEntityConfiguration<PlaylistQuality>, IEntityMap
    {
        public override void Configure(EntityTypeBuilder<PlaylistQuality> entity)
        {
            entity.ToTable("T_Playlist_Quality", "dbo");
            entity.Property(p => p.Id).HasColumnName("Id");
            entity.Property(p => p.Id).ValueGeneratedOnAdd();
        }
    }
}
