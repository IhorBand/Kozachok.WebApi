using Kozachok.Shared.Abstractions.Commands;
using Kozachok.Shared.DTO.Common;

namespace Kozachok.Domain.Queries.Room
{
    public class GetJoinedRoomsQuery : RequestCommand<PagedResult<Shared.DTO.Models.DbEntities.Room>>
    {
        public int? Page { get; set; }
        public int? ItemsPerPage { get; set; }
        public string Name { get; set; }
    }
}
