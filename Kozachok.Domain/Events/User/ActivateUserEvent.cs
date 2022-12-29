using Kozachok.Shared.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kozachok.Domain.Events.User
{
    public class ActivateUserEvent : Event
    {
        public ActivateUserEvent(Guid userId, string name, string email, string confirmationCode)
        {
            UserId = userId;
            Name = name;
            Email = email;
            ConfirmationCode = confirmationCode;

            AggregateId = UserId;
        }

        public Guid UserId { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string ConfirmationCode { get; private set; }
    }
}
