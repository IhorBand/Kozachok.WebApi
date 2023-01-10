using Kozachok.Shared.Abstractions.Commands;
using Kozachok.Shared.DTO.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Kozachok.Domain.Commands.Room
{
    public class UpdateRoomCommand : Command
    {
        public Guid RoomId { get; set; }
        [MaxLength(500)]
        public string Name { get; set; }
        public RoomType RoomType { get; set; }
    }
}
