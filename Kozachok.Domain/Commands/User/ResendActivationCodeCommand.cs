using Kozachok.Shared.Abstractions.Commands;
using Kozachok.Shared.DTO.Models.Result;
using System.ComponentModel.DataAnnotations;

namespace Kozachok.Domain.Commands.User
{
    public class ResendActivationCodeCommand : Command
    {
        [MaxLength(200)]
        public string Email { get; set; }
    }
}
