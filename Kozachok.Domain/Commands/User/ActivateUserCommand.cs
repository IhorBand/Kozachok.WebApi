using Kozachok.Shared.Abstractions.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kozachok.Domain.Commands.User
{
    public class ActivateUserCommand : Command
    {
        public string ConfirmationCode { get; set; }
    }
}
