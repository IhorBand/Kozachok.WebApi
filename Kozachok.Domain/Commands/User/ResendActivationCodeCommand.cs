using Kozachok.Shared.Abstractions.Commands;
using Kozachok.Shared.DTO.Models.Result;

namespace Kozachok.Domain.Commands.User
{
    public class ResendActivationCodeCommand : Command
    {
        public string Email { get; set; }
    }
}
