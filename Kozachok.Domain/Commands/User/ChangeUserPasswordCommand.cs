using System;
using Kozachok.Shared.Abstractions.Commands;

namespace Kozachok.Domain.Commands.User
{
    public class ChangeUserPasswordCommand : Command
    {
        public Guid Id { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
    }
}
