using Kozachok.Shared.DTO.Common;
using System;

namespace Kozachok.Shared.DTO.Models.DbEntities
{
    public class ChatConnection : Entity
    {
        // -> Empty contructor for EF
        public ChatConnection()
        {

        }

        public ChatConnection(
            Guid userId,
            Guid roomId,
            string connectionId)
        {
            UserId = userId;
            RoomId = roomId;
            ConnectionId = connectionId;
            CreatedDateUTC = DateTime.UtcNow;
        }

        public virtual Guid RoomId { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual string ConnectionId { get; set; }
        public virtual DateTime CreatedDateUTC { get; private set; }
    }
}
