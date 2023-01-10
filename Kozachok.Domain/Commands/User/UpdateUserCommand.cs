using Kozachok.Shared.Abstractions.Commands;
using System;
using System.ComponentModel.DataAnnotations;

namespace Kozachok.Domain.Commands.User
{
    public class UpdateUserCommand : Command
    {
        [MaxLength(200)]
        public string Name { get; set; }
    }
}
