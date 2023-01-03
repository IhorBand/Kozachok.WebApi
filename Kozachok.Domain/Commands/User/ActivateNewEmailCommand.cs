using Kozachok.Shared.Abstractions.Commands;

namespace Kozachok.Domain.Commands.User
{
    public class ActivateNewEmailCommand : Command
    {
        public string ChangeEmailCode { get; set; }
    }
}
