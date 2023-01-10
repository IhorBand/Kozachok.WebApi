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
            RoomType roomTypeId)
        {
            Name = name;
            RoomTypeId = roomTypeId;
            CreatedDateUTC = DateTime.UtcNow;
        }

        public virtual string Name { get; set; }
        public virtual RoomType RoomTypeId { get; set; }
        public virtual DateTime CreatedDateUTC { get; private set; }
    }
}
