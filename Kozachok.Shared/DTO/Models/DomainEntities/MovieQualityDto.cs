using Kozachok.Shared.DTO.Enums;
using System;

namespace Kozachok.Shared.DTO.Models.DomainEntities
{
    public class MovieQualityDto
    {
        public Guid Id { get; set; }
        public VideoQualityType QualityId { get; set; }
        public string MovieUrl { get; set; }
    }
}
