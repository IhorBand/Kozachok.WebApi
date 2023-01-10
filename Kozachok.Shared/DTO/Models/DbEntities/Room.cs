using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Enums;
using System;

namespace Kozachok.Shared.DTO.Models.DbEntities
{
    public class Room : Entity
    {
        // -> Empty contructor for EF
        public Room()
        {

        }

        public Room(
            string name,
            RoomType roomTypeId,
            Guid ownerUserId)
        {
            Name = name;
            RoomTypeId = roomTypeId;
            OwnerUserId = ownerUserId;
            CreatedDateUTC = DateTime.UtcNow;
        }

        public virtual string Name { get; set; }
        public virtual RoomType RoomTypeId { get; set; }
        public virtual Guid OwnerUserId { get; set; }
        public virtual DateTime CreatedDateUTC { get; private set; }
    }
}
