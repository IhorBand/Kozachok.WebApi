using Kozachok.Domain.Handlers.Notifications;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.Abstractions.Identity;
using Kozachok.Shared.DTO.Common;
using MediatR;
using System;

namespace Kozachok.Domain.Handlers.Common
{
    public abstract class QueryHandler
    {
        private readonly DomainNotificationHandler notifications;
        protected readonly IMediatorHandler Bus;

        protected QueryHandler(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications)
        {
            this.Bus = bus;
            this.notifications = (DomainNotificationHandler)notifications;
        }

        protected bool IsValidOperation() => !notifications.HasNotifications();

        protected bool IsUserAuthorized(IUser user)
        {
            return user?.Id != null
                   && (user.Id == null || user.Id != Guid.Empty);
        }
    }
}
