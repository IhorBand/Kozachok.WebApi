using System;

namespace Kozachok.Shared.DTO.Common
{
    public abstract class Entity
    {
        public virtual Guid Id { get; set; }
    }
}
