using Kozachok.Shared.Abstractions.Events;
using System;

namespace Kozachok.Shared.Abstractions.Commands
{
    public abstract class Command : Message
    {
        public DateTime DateTime { get; private set; }

        protected Command()
        {
            DateTime = DateTime.Now;
        }
    }
}
