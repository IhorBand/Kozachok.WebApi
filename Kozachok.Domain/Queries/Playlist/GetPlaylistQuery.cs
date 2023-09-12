using Kozachok.Shared.Abstractions.Commands;
using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Models.DomainEntities;
using System;

namespace Kozachok.Domain.Queries.Playlist
{
    public class GetPlaylistQuery : RequestCommand<PagedResult<PlaylistMovieDto>>
    {
        public int? Page { get; set; }
        public int? ItemsPerPage { get; set; }
        public Guid RoomId { get; set; }
    }
}
