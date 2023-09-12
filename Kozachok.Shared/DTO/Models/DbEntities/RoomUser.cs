using Kozachok.Shared.DTO.Common;
using System;

namespace Kozachok.Shared.DTO.Models.DbEntities
{
    public class RoomUser : Entity
    {
        public virtual Guid RoomId { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual bool IsOwner { get; set; }
        public virtual DateTime CreatedDateUtc { get; private set; }

        public virtual User User { get; set; }
        public virtual Room Room { get; set; }

        public static RoomUser Create(
            Guid userId,
            Guid roomId,
            bool isOwner)
        {
            return new RoomUser
            {
                UserId = userId,
                RoomId = roomId,
                IsOwner = isOwner,
                CreatedDateUtc = DateTime.UtcNow
            };
        }
    }
}
