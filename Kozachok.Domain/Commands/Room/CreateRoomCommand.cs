using Kozachok.Shared.Abstractions.Commands;
using Kozachok.Shared.DTO.Enums;
using System.ComponentModel.DataAnnotations;

namespace Kozachok.Domain.Commands.Room
{
    public class CreateRoomCommand : RequestCommand<Shared.DTO.Models.DbEntities.Room>
    {
        [MaxLength(500)]
        public string Name { get; set; }
        public RoomType RoomType { get; set; }
    }
}
