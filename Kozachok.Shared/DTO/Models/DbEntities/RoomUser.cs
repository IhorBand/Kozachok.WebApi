using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kozachok.Shared.DTO.Models.DbEntities
{
    public class RoomUser : Entity
    {
        // -> Empty contructor for EF
        public RoomUser()
        {

        }

        public RoomUser(
            Guid userId,
            Guid roomId,
            bool isOwner)
        {
            UserId = userId;
            RoomId = roomId;
            IsOwner = isOwner;
            CreatedDateUTC = DateTime.UtcNow;
        }

        public virtual Guid RoomId { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual bool IsOwner { get; set; }
        public virtual DateTime CreatedDateUTC { get; private set; }
    }
}
