using Kozachok.Shared.Abstractions.Commands;
using System;

namespace Kozachok.Domain.Commands.User
{
    public class UpdateUserCommand : Command
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
