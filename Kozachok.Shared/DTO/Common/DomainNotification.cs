using Kozachok.Shared.Abstractions.Events;

namespace Kozachok.Shared.DTO.Common
{
    public class DomainNotification : Event
    {
        public string Message { get; private set; }

        public DomainNotification(string message)
        {
            Message = message;
        }
    }
}
