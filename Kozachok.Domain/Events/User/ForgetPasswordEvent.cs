using Kozachok.Shared.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kozachok.Domain.Events.User
{
    public class ForgetPasswordEvent : Event
    {
        public ForgetPasswordEvent(Guid userId, string name, string email, string forgetPasswordCode)
        {
            UserId = userId;
            Name = name;
            Email = email;
            ForgetPasswordCode = forgetPasswordCode;

            AggregateId = UserId;
        }

        public Guid UserId { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string ForgetPasswordCode { get; private set; }
    }
}
