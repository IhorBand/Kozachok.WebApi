using System;

namespace Kozachok.Shared.DTO.Models.DomainEntities
{
    public class UserInformationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ThumbnailImageUrl { get; set; }
    }
}
