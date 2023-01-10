using Kozachok.Shared.Abstractions.Commands;
using System;

namespace Kozachok.Domain.Queries.Room
{
    public class GetRoomQuery : RequestCommand<Shared.DTO.Models.DbEntities.Room>
    {
        public Guid RoomId { get; set; }
    }
}
