using Kozachok.Repository.Config;
using Kozachok.Shared.Abstractions.Mapping;
using Kozachok.Shared.DTO.Models.DbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kozachok.Repository.Mapping
{
    internal class PlaylistMovieVideoQualityMap : DbEntityConfiguration<PlaylistMovieVideoQuality>, IEntityMap
    {
        public override void Configure(EntityTypeBuilder<PlaylistMovieVideoQuality> entity)
        {
            entity.ToTable("T_Playlist_Movie_Video_Quality", "dbo");
            entity.Property(p => p.Id).HasColumnName("Id");
            entity.Property(p => p.Id).ValueGeneratedOnAdd();


            entity.HasOne(x => x.PlaylistMovieVideo)
                .WithMany(x => x.PlaylistMovieVideoQualities);

            entity.HasOne(x => x.PlaylistMovie);
        }
    }
}
