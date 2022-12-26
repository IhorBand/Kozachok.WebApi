using System;
using Kozachok.Shared.Abstractions.Events;

namespace Kozachok.Domain.Events.User
{
    public class ChangeUserPasswordEvent : Event
    {
        public ChangeUserPasswordEvent(Guid id, string password)
        {
            Id = id;
            Password = password;

            AggregateId = Id;
        }

        public Guid Id { get; private set; }
        public string Password { get; private set; }
    }
}
