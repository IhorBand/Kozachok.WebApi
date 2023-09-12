using Kozachok.Shared.Abstractions.Commands;
using System;

namespace Kozachok.Domain.Queries.Room
{
    public class GetRoomFullInformationDtoQuery : RequestCommand<Shared.DTO.Models.DomainEntities.RoomFullInformationDto>
    {
        public Guid RoomId { get; set; }
    }
}
