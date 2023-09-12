using Kozachok.Shared.Abstractions.Commands;
using Kozachok.Shared.DTO.Models.DomainEntities;
using System;

namespace Kozachok.Domain.Queries.Room
{
    public class GetRoomQuery : RequestCommand<RoomDto>
    {
        public Guid RoomId { get; set; }
    }
}
