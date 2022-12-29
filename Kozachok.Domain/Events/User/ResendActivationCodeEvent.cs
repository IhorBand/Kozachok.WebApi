using Kozachok.Shared.Abstractions.Events;
using Kozachok.Shared.DTO.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kozachok.Domain.Events.User
{
    public class ResendActivationCodeEvent : Event
    {
        public ResendActivationCodeEvent(Guid id, string name, string email, string confirmationCode)
        {
            Id = id;
            Name = name;
            Email = email;
            ConfirmationCode = confirmationCode;
            AggregateId = Id;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string ConfirmationCode { get; private set; }
    }
}
