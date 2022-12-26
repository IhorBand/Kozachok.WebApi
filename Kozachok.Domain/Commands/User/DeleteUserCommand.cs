using Kozachok.Shared.Abstractions.Commands;
using System;

namespace Kozachok.Domain.Commands.User
{
    public class DeleteUserCommand : Command
    {
        public DeleteUserCommand(Guid id) => this.Id = id;

        public Guid Id { get; set; }
    }
}
