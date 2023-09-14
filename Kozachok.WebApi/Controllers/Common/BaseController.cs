using AutoMapper;
using Kozachok.Domain.Handlers.Notifications;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.DTO.Common;
using Kozachok.WebApi.Models.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kozachok.WebApi.Controllers.Common
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        private readonly DomainNotificationHandler notifications;
        protected readonly IMapper Mapper;

        protected readonly IMediatorHandler Bus;

        protected BaseController(IMediatorHandler bus, IMapper mapper, INotificationHandler<DomainNotification> notifications)
        {
            this.Bus = bus;
            this.Mapper = mapper;
            this.notifications = (DomainNotificationHandler)notifications;
        }

        protected IEnumerable<DomainNotification> Notifications => notifications.GetNotifications();

        protected bool IsValidOperation() => !notifications.HasNotifications();

        protected new IActionResult Response<T>(object? result)
        {
            if (result == null)
            {
                return Response();
            }

            var model = Mapper.Map<T>(result);
            return Response(model);

        }

        protected new IActionResult Response(object? result = null)
        {
            if (IsValidOperation())
            {
                if (result == null)
                    return Ok();

                return Ok(result);
            }

            var errors = this.Mapper.Map<ErrorMessage[]>(Notifications);

            return BadRequest(errors);
        }

        protected string GetClaimValue(string claimName)
        {
            var claim = this.User.Claims.FirstOrDefault(c => c.Type == claimName);
            return claim != null ? claim.Value : string.Empty;
        }

        protected string? GetClaim(string claimType) => User?.Claims?.FirstOrDefault(x => x.Type == claimType)?.Value;
    }
}
