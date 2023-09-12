using Kozachok.Shared.DTO.Common;
using System;

namespace Kozachok.Shared.DTO.Models.DbEntities
{
    public class ChatConnection : Entity
    {
        public virtual Guid RoomId { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual string ConnectionId { get; set; }
        public virtual DateTime CreatedDateUtc { get; private set; }

        public static ChatConnection Create(
            Guid userId,
            Guid roomId,
            string connectionId)
        {
            return new ChatConnection
            {
                UserId = userId,
                RoomId = roomId,
                ConnectionId = connectionId,
                CreatedDateUtc = DateTime.UtcNow
            };
        }
    }
}
