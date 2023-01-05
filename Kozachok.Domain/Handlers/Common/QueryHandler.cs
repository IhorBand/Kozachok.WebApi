using Kozachok.Domain.Handlers.Notifications;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.Abstractions.Identity;
using Kozachok.Shared.Abstractions.Repositories.Common;
using Kozachok.Shared.DTO.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kozachok.Domain.Handlers.Common
{
    public abstract class QueryHandler
    {
        private readonly DomainNotificationHandler notifications;
        protected readonly IMediatorHandler bus;

        public QueryHandler(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications)
        {
            this.bus = bus;
            this.notifications = (DomainNotificationHandler)notifications;
        }

        protected bool IsValidOperation() => !notifications.HasNotifications();

        protected bool IsUserAuthorized(IUser user)
        {
            if (user == null 
                || (user != null && user.Id == null) 
                || (user != null && user.Id != null && user.Id == Guid.Empty))
                return false;
            return true;
        }
    }
}
