using Kozachok.Shared.Abstractions.Commands;

namespace Kozachok.Domain.Commands.User
{
    public class SendForgetPasswordEmailCommand : Command
    {
        public string Email { get; set; }
    }
}
