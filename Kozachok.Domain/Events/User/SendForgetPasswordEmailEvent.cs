using Kozachok.Shared.Abstractions.Events;
using System;

namespace Kozachok.Domain.Events.User
{
    public class SendForgetPasswordEmailEvent : Event
    {
        public SendForgetPasswordEmailEvent(Guid userId, string name, string email, string forgetPasswordCode)
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
