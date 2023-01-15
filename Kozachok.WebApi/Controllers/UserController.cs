using AutoMapper;
using Kozachok.Domain.Commands.User;
using Kozachok.Domain.Queries.User;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Models.Result.Email;
using Kozachok.Shared.DTO.Models.Result.User;
using Kozachok.WebApi.Auth;
using Kozachok.WebApi.Controllers.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kozachok.WebApi.Controllers
{
    public class UserController : UserControllerBase
    {
        public UserController(
            IMediatorHandler mediator,
            IMapper mapper,
            INotificationHandler<DomainNotification> notifications
            ) : base(mediator, mapper, notifications)
        {
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateUserCommand command)
        {
            var result = await bus.RequestAsync<EmailTimer>(command);
            return Response(result);
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

        [HttpPut("ResendConfirmationCode")]
        public async Task<IActionResult> ResendActivationCode([FromBody] ResendActivationCodeCommand command)
        {
            var result = await bus.RequestAsync<EmailTimer>(command);
            return Response(result);
        }

        [HttpPut("Activate")]
        public async Task<IActionResult> Activate([FromBody] ActivateUserCommand command)
        {
            await bus.SendAsync(command);
            return Response();
        }

        [HttpPut("SendForgetPasswordEmail")]
        public async Task<IActionResult> SendForgetPasswordEmail([FromBody] SendForgetPasswordEmailCommand command)
        {
            var result = await bus.RequestAsync<EmailTimer>(command);
            return Response(result);
        }

        [HttpPut("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordCommand command)
        {
            await bus.SendAsync(command);
            return Response();
        }

        [HttpPut("SendChangeEmailConfirmation")]
        [BearerAuthorization()]
        public async Task<IActionResult> SendChangeEmailConfirmation([FromBody] SendChangeEmailConfirmationCommand command)
        {
            var result = await bus.RequestAsync<EmailTimer>(command);
            return Response(result);
        }

        [HttpPut("ActivateNewEmail")]
        [BearerAuthorization()]
        public async Task<IActionResult> ActivateNewEmail([FromBody] ActivateNewEmailCommand command)
        {
            await bus.SendAsync(command);
            return Response();
        }

        [HttpGet("{userId}")]
        [BearerAuthorization]
        public async Task<IActionResult> GetUser([FromRoute(Name = "userId")] Guid userId)
        {
            var result = await bus.RequestAsync<UserDetails>(new GetUserDetailQuery() { UserId = userId });
            return Response<Models.User.UserDetails>(result);
        }
    }
}
