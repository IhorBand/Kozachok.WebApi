using System.ComponentModel.DataAnnotations;
using Kozachok.Shared.Abstractions.Commands;

namespace Kozachok.Domain.Commands.User
{
    public class ChangeUserPasswordCommand : Command
    {
        public string OldPassword { get; set; }
        [MaxLength(200)]
        public string Password { get; set; }
        [MaxLength(200)]
        public string PasswordConfirmation { get; set; }
    }
}
