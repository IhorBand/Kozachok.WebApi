using AutoMapper;
using Kozachok.Domain.Commands.User;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.DTO.Common;
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
            await bus.SendAsync(command);
            return Response();
        }

        [HttpPost("UpdateThumbnailImage")]
        [BearerAuthorization()]
        public async Task<IActionResult> UpdateThumbnailImage(IFormFile file)
        {
            var result = await bus.RequestAsync(new UpdateUserThumbnailImageCommand() { File = file });

            if (result != null)
            {
                var model = mapper.Map<Models.File.File>(result);
                return Response(model);
            }

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

        [HttpPut("ResendConfirmationCode")]
        public async Task<IActionResult> ResendActivationCode([FromBody] ResendActivationCodeCommand command)
        {
            await bus.SendAsync(command);
            return Response();
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
            await bus.SendAsync(command);
            return Response();
        }

        [HttpPut("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordCommand command)
        {
            await bus.SendAsync(command);
            return Response();
        }
    }
}
