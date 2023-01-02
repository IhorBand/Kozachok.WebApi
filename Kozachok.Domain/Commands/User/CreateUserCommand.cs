using Kozachok.Shared.Abstractions.Commands;
using System;

namespace Kozachok.Domain.Commands.User
{
    public class CreateUserCommand : Command
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
        public Guid? ThumbnailImageFileId { get; set; }
    }
}
