using Kozachok.Shared.Abstractions.Commands;
using Kozachok.Shared.DTO.Models.Result.User;
using System;

namespace Kozachok.Domain.Queries.User
{
    public class GetUserDetailQuery : RequestCommand<UserDetails>
    {
        public Guid UserId { get; set; }
    }
}
