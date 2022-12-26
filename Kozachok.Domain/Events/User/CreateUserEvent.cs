using Kozachok.Shared.Abstractions.Events;
using System;

namespace Kozachok.Domain.Events.User
{
    public class CreateUserEvent : Event
    {
        public CreateUserEvent(Guid id, string name, string email, string password)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;

            AggregateId = Id;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
    }
}
