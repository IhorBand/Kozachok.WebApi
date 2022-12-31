using Kozachok.Shared.Abstractions.Events;
using System;

namespace Kozachok.Shared.DTO.Common
{
    public class DomainNotification : Event
    {
        public string Message { get; private set; }

        public DomainNotification(string message)
        {
            Message = message;
            AggregateId = Guid.NewGuid();
        }
    }
}
