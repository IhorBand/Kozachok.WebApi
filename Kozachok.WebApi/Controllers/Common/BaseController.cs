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
        protected readonly IMapper mapper;

        protected readonly IMediatorHandler bus;

        protected BaseController(IMediatorHandler bus, IMapper mapper, INotificationHandler<DomainNotification> notifications)
        {
            this.bus = bus;
            this.mapper = mapper;
            this.notifications = (DomainNotificationHandler)notifications;
        }

        protected IEnumerable<DomainNotification> Notifications => notifications.GetNotifications();

        protected bool IsValidOperation() => !notifications.HasNotifications();

        protected new IActionResult Response<T>(object? result)
        {
            if(result != null)
            {
                var model = mapper.Map<T>(result);
                return Response(model);
            }

            return Response();
        }

        protected new IActionResult Response(object? result = null)
        {
            if (IsValidOperation())
            {
                if (result == null)
                    return Ok();

                return Ok(result);
            }

            var errors = this.mapper.Map<ErrorMessage[]>(Notifications);

            return BadRequest(errors);
        }

        protected string? GetClaimValue(string claimName)
        {
            if (this.User != null)
            {
                var claim = this.User.Claims.FirstOrDefault(c => c.Type == claimName);
                if (claim != null)
                {
                    return claim.Value;
                }
            }

            return string.Empty;
        }

        protected string? GetClaim(string claimType) => User?.Claims?.FirstOrDefault(x => x.Type == claimType)?.Value;
    }
}
