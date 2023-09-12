using System;

namespace Kozachok.Shared.DTO.Models.DomainEntities
{
    public class ChatConnectionDto
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public Guid UserId { get; set; }
        public string ConnectionId { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        //public RoomDto RoomDto { get; set; }
        //public UserInformationDTO UserInformationDTO { get; set; }
    }
}
