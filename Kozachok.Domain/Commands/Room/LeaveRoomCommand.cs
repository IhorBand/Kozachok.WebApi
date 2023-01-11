using Kozachok.Shared.Abstractions.Commands;
using System;

namespace Kozachok.Domain.Commands.Room
{
    public class LeaveRoomCommand : Command
    {
        public Guid RoomId { get; set; }
    }
}
