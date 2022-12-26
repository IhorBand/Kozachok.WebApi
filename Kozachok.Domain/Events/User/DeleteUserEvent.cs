using Kozachok.Shared.Abstractions.Events;
using System;

namespace Kozachok.Domain.Events.User
{
    public class DeleteUserEvent : Event
    {
        public DeleteUserEvent(Guid id)
        {
            Id = id;

            AggregateId = Id;
        }

        public Guid Id { get; private set; }
    }
}
