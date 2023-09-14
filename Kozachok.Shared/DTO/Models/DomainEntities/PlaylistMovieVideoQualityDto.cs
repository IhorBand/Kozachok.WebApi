using Kozachok.Shared.DTO.Enums;
using System;

namespace Kozachok.Shared.DTO.Models.DomainEntities
{
    public class PlaylistMovieVideoQualityDto
    {
        public Guid Id { get; set; }
        public Guid PlaylistMovieId { get; set; }
        public Guid PlaylistMovieVideoId { get; set; }
        public VideoQualityType QualityId { get; set; }
        public string MovieUrl { get; set; }
    }
}
