using Kozachok.Shared.Abstractions.Commands;
using Kozachok.Shared.DTO.Models.Result.Email;
using System.ComponentModel.DataAnnotations;

namespace Kozachok.Domain.Commands.User
{
    public class CreateUserCommand : RequestCommand<EmailTimer>
    {
        [MaxLength(200)]
        public string Name { get; set; }
        
        [MaxLength(200)]
        public string Email { get; set; }

        [MaxLength(200)]
        public string Password { get; set; }

        [MaxLength(200)]
        public string PasswordConfirmation { get; set; }
    }
}
