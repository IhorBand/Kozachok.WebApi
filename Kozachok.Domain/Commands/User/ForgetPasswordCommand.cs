using Kozachok.Shared.Abstractions.Commands;
using System.ComponentModel.DataAnnotations;

namespace Kozachok.Domain.Commands.User
{
    public class ForgetPasswordCommand : Command
    {
        public string ForgetPasswordCode { get; set; }
        [MaxLength(200)]
        public string Password { get; set; }
        [MaxLength(200)]
        public string PasswordConfirmation { get; set; }
    }
}
