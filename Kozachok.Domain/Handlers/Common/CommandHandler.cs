using Kozachok.Domain.Handlers.Notifications;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.Abstractions.Identity;
using Kozachok.Shared.Abstractions.Repositories.Common;
using Kozachok.Shared.DTO.Common;
using MediatR;
using System;

namespace Kozachok.Domain.Handlers.Common
{
    public abstract class CommandHandler
    {
        private readonly IUnitOfWork uow;
        private readonly DomainNotificationHandler notifications;
        protected readonly IMediatorHandler Bus;

        public CommandHandler(IUnitOfWork uow, IMediatorHandler bus, INotificationHandler<DomainNotification> notifications)
        {
            this.uow = uow;
            this.Bus = bus;
            this.notifications = (DomainNotificationHandler)notifications;
        }

        protected bool IsValidOperation() => !notifications.HasNotifications();

        protected bool IsUserAuthorized(IUser user)
        {
            return user?.Id != null
                   && (user.Id == null || user.Id != Guid.Empty);
        }

        public void Commit()
        {
            if (IsValidOperation())
                uow.Commit();
        }
    }
}
