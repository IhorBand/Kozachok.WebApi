﻿using Kozachok.Shared.Abstractions.Commands;
using System;

namespace Kozachok.Domain.Commands.ChatConnection
{
    public class JoinRoomCommand : Command
    {
        public Guid RoomId { get; set; }
        public Guid UserId { get; set; }
        public string ConnectionId { get; set; }
    }
}
