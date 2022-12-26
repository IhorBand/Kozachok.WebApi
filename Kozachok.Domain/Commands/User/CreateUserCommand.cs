using Kozachok.Shared.Abstractions.Commands;

namespace Kozachok.Domain.Commands.User
{
    public class CreateUserCommand : Command
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
    }
}
