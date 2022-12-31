using Kozachok.Shared.Abstractions.Commands;

namespace Kozachok.Domain.Commands.User
{
    public class ForgetPasswordCommand : Command
    {
        public string ForgetPasswordCode { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
    }
}
