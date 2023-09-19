using Kozachok.Shared.DTO.Configuration;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.DTO.Common;
using MediatR;
using AutoMapper;
using Kozachok.Shared.Abstractions.Identity;

namespace Kozachok.WebApi.Controllers.Common
{
    public class UserControllerBase : BaseController
    {
        protected UserControllerBase(IMediatorHandler bus, IMapper mapper, INotificationHandler<DomainNotification> notifications, IUser? user = null) : base(bus, mapper, notifications)
        {
            CurrentUser = user;
        }

        protected IUser? CurrentUser;

        protected Guid UserId
        {
            get
            {
                var idStr = GetClaimValue(JwtCustomClaimNames.UserId);
                return new Guid(idStr);
            }
        }

        protected string? Email => GetClaimValue(JwtCustomClaimNames.Email);
        protected string? UserName => GetClaimValue(JwtCustomClaimNames.UserName);

        protected bool IsUserAuthorized()
        {
            return CurrentUser?.Id != null
                   && (CurrentUser.Id == null || CurrentUser.Id != Guid.Empty);
        }
    }
}
