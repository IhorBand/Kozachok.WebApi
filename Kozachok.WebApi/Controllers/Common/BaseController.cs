using Kozachok.Domain.Handlers.Notifications;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.DTO.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kozachok.WebApi.Controllers.Common
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        private readonly DomainNotificationHandler notifications;
        protected readonly IMediatorHandler bus;

        protected BaseController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications)
        {
            this.bus = bus;
            this.notifications = (DomainNotificationHandler)notifications;
        }

        protected IEnumerable<DomainNotification> Notifications => notifications.GetNotifications();

        protected bool IsValidOperation() => !notifications.HasNotifications();

        protected new IActionResult Response(object? result = null)
        {
            if (IsValidOperation())
            {
                if (result == null)
                    return Ok();

                return Ok(result);
            }

            return BadRequest(Notifications);
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
