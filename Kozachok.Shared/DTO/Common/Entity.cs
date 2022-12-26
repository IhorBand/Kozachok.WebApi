using System;
using System.Collections.Generic;
using System.Text;

namespace Kozachok.Shared.DTO.Common
{
    public abstract class Entity
    {
        public virtual Guid Id { get; set; }
    }
}
