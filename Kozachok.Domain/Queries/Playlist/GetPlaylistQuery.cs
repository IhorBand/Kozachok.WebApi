using Kozachok.Shared.Abstractions.Commands;
using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Models.Result.Playlist;
using System;

namespace Kozachok.Domain.Queries.Playlist
{
    public class GetPlaylistQuery : RequestCommand<PagedResult<PlaylistMovies>>
    {
        public int? Page { get; set; }
        public int? ItemsPerPage { get; set; }
        public Guid RoomId { get; set; }
    }
}
