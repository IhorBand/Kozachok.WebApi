using Kozachok.Repository.Config;
using Kozachok.Shared.Abstractions.Mapping;
using Kozachok.Shared.DTO.Models.DbEntities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Kozachok.Repository.Mapping
{
    internal class PlaylistMovieVideoMap : DbEntityConfiguration<PlaylistMovieVideo>, IEntityMap
    {
        public override void Configure(EntityTypeBuilder<PlaylistMovieVideo> entity)
        {
            entity.ToTable("T_Playlist_Movie_Video", "dbo");
            entity.Property(p => p.Id).HasColumnName("Id");
            entity.Property(p => p.Id).ValueGeneratedOnAdd();


            entity.HasOne(x => x.PlaylistMovie)
                .WithMany(x => x.PlaylistMovieVideos);

            entity.HasMany(x => x.PlaylistMovieVideoQualities)
                .WithOne(x => x.PlaylistMovieVideo);
        }
    }
}
