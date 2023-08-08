using System.Collections.Generic;

namespace Kozachok.Shared.DTO.Models.Result.Playlist
{
    public class PlaylistMovies
    {
        public DbEntities.PlaylistMovie MovieDescription { get; set; }
        public List<DbEntities.PlaylistQuality> MovieQualities { get; set; }
    }
}
