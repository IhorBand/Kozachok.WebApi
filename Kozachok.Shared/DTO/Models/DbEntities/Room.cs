using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Enums;
using System;
using System.Collections.Generic;

namespace Kozachok.Shared.DTO.Models.DbEntities
{
    public class Room : Entity
    {
        public virtual string Name { get; set; }
        public virtual RoomType RoomTypeId { get; set; }
        public virtual Guid OwnerUserId { get; set; }
        public virtual DateTime CreatedDateUtc { get; private set; }

        public virtual ICollection<RoomUser> RoomUsers { get; set; }
        public virtual ICollection<PlaylistMovie> PlaylistMovies { get; set; }

        public static Room Create(
            string name,
            RoomType roomTypeId,
            Guid ownerUserId)
        {
            return new Room
            {
                Name = name,
                RoomTypeId = roomTypeId,
                OwnerUserId = ownerUserId,
                CreatedDateUtc = DateTime.UtcNow
            };
        }
    }
}
