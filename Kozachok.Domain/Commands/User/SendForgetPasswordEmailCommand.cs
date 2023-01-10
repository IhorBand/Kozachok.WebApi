using Kozachok.Shared.Abstractions.Commands;
using System.ComponentModel.DataAnnotations;

namespace Kozachok.Domain.Commands.User
{
    public class SendForgetPasswordEmailCommand : Command
    {
        [MaxLength(200)]
        public string Email { get; set; }
    }
}
