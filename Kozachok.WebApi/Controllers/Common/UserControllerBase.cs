using Kozachok.Shared.DTO.Configuration;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.DTO.Common;
using MediatR;
using AutoMapper;

namespace Kozachok.WebApi.Controllers.Common
{
    public class UserControllerBase : BaseController
    {
        protected UserControllerBase(IMediatorHandler bus, IMapper mapper, INotificationHandler<DomainNotification> notifications) : base(bus, mapper, notifications)
        {
        }

        protected Guid UserId
        {
            get
            {
                var idStr = this.GetClaimValue(JwtCustomClaimNames.UserId);
                return new Guid(idStr);
            }
        }

        protected string? Email => this.GetClaimValue(JwtCustomClaimNames.Email);
        protected string? UserName => this.GetClaimValue(JwtCustomClaimNames.UserName);
    }
}
