using Kozachok.Shared.Abstractions.Commands;
using System;

namespace Kozachok.Domain.Commands.Room
{
    public class JoinRoomCommand : Command
    {
        public Guid RoomId { get; set; }
        public string SecretToken { get; set; }
    }
}
