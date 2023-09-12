using Kozachok.Shared.DTO.Enums;
using System;

namespace Kozachok.Shared.DTO.Models.DomainEntities
{
    public class RoomDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public RoomType RoomTypeId { get; set; }
        public Guid OwnerUserId { get; set; }
        public DateTime CreatedDateUtc { get; set; }
    }
}
