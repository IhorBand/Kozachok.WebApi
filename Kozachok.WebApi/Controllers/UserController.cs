using Kozachok.Domain.Commands.User;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Common;
using Kozachok.WebApi.Auth;
using Kozachok.WebApi.Controllers.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kozachok.WebApi.Controllers
{
    public class UserController : UserControllerBase
    {
        public UserController(
            IMediatorHandler mediator, 
            INotificationHandler<DomainNotification> notifications
            ) : base(mediator, notifications)
        {
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateUserCommand command)
        {
            await bus.SendAsync(command);
            return Response();
        }

        [HttpPut()]
        [BearerAuthorization()]
        public async Task<IActionResult> Put([FromBody] UpdateUserCommand command)
        {
            await bus.SendAsync(command);
            return Response();
        }

        [HttpPut("ChangePassword")]
        [BearerAuthorization()]
        public async Task<IActionResult> ChangePassword([FromBody] ChangeUserPasswordCommand command)
        {
            await bus.SendAsync(command);
            return Response();
        }
    }
}
