using Kozachok.Shared.Abstractions.Commands;
using Kozachok.Shared.DTO.Enums;
using Kozachok.Shared.DTO.Models.DomainEntities;
using System.ComponentModel.DataAnnotations;

namespace Kozachok.Domain.Commands.Room
{
    public class CreateRoomCommand : RequestCommand<RoomDto>
    {
        [MaxLength(500)]
        public string Name { get; set; }
        public RoomType RoomType { get; set; }
    }
}
