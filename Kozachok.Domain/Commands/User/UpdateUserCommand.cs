﻿using Kozachok.Shared.Abstractions.Commands;
using System;

namespace Kozachok.Domain.Commands.User
{
    public class UpdateUserCommand : Command
    {
        public string Name { get; set; }
    }
}
