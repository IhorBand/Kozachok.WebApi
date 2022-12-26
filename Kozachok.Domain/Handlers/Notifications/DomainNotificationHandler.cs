using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Kozachok.Shared.DTO.Common;

namespace Kozachok.Domain.Handlers.Notifications
{
    public class DomainNotificationHandler : INotificationHandler<DomainNotification>
    {
        private readonly List<DomainNotification> notifications = new List<DomainNotification>();

        public Task Handle(DomainNotification message, CancellationToken cancellationToken)
        {
            notifications.Add(message);
            return Task.CompletedTask;
        }

        public virtual List<DomainNotification> GetNotifications() => notifications;

        public virtual bool HasNotifications() => GetNotifications().Any();

        public void Dispose() => notifications.Clear();
    }
}
