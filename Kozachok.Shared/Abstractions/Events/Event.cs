using MediatR;
using System;

namespace Kozachok.Shared.Abstractions.Events
{
    public abstract class Event : Message, INotification
    {
        public DateTime DateTime { get; private set; }

        protected Event()
        {
            DateTime = DateTime.UtcNow;
        }
    }
}
