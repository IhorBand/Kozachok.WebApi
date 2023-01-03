using Kozachok.Shared.Abstractions.Commands;

namespace Kozachok.Domain.Commands.User
{
    public class SendChangeEmailConfirmationCommand : Command
    {
        public string Email { get; set; }
    }
}
