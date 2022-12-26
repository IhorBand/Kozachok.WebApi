using Kozachok.Domain.Handlers.Notifications;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.Abstractions.Repositories.Common;
using Kozachok.Shared.DTO.Common;
using MediatR;

namespace Kozachok.Domain.Handlers.Common
{
    public abstract class CommandHandler
    {
        private readonly IUnitOfWork uow;
        private readonly DomainNotificationHandler notifications;
        protected readonly IMediatorHandler bus;

        public CommandHandler(IUnitOfWork uow, IMediatorHandler bus, INotificationHandler<DomainNotification> notifications)
        {
            this.uow = uow;
            this.bus = bus;
            this.notifications = (DomainNotificationHandler)notifications;
        }

        protected bool IsValidOperation() => !notifications.HasNotifications();

        public void Commit()
        {
            if (IsValidOperation())
                uow.Commit();
        }
    }
}
