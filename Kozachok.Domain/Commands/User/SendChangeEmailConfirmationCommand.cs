using Kozachok.Shared.Abstractions.Commands;
using Kozachok.Shared.DTO.Models.Result.Email;
using System.ComponentModel.DataAnnotations;

namespace Kozachok.Domain.Commands.User
{
    public class SendChangeEmailConfirmationCommand : RequestCommand<EmailTimer>
    {
        [MaxLength(200)]
        public string Email { get; set; }
    }
}
