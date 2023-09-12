using System;
using System.Threading;
using System.Threading.Tasks;
using Kozachok.Shared.Abstractions.Repositories.Common;
using Kozachok.Shared.DTO.Models.DbEntities;
using Kozachok.Shared.DTO.Models.DomainEntities;

namespace Kozachok.Shared.Abstractions.Repositories
{
    public interface IRoomRepository : ICrudRepository<Room>
    {
        public Task<RoomFullInformationDto> GetRoomFullInformationDtoAsync(Guid roomId, CancellationToken ct = default);
    }
}
