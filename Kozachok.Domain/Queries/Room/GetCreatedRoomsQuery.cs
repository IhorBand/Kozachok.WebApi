using Kozachok.Shared.Abstractions.Commands;
using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Models.DomainEntities;

namespace Kozachok.Domain.Queries.Room
{
    public class GetCreatedRoomsQuery : RequestCommand<PagedResult<RoomDto>>
    {
        public int? Page { get; set; }
        public int? ItemsPerPage { get; set; }
        public string Name { get; set; }
    }
}
