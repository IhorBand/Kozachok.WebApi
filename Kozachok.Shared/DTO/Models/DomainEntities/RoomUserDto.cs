using System;

namespace Kozachok.Shared.DTO.Models.DomainEntities
{
    public class RoomUserDto
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public Guid UserId { get; set; }
        public bool IsOwner { get; set; }
        public DateTime CreatedDateUtc { get; set; }
    }
}
