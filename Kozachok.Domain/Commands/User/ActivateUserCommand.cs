using Kozachok.Shared.Abstractions.Commands;

namespace Kozachok.Domain.Commands.User
{
    public class ActivateUserCommand : Command
    {
        public string ConfirmationCode { get; set; }
    }
}
