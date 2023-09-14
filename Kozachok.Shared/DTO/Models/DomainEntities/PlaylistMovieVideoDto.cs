using System;
using System.Collections.Generic;

namespace Kozachok.Shared.DTO.Models.DomainEntities
{
    public class PlaylistMovieVideoDto
    {
        public Guid Id { get; set; }
        public Guid PlaylistMovieId { get; set; }
        public int Season { get; set; }
        public int Episode { get; set; }
        public string TranslatorExternalId { get; set; }
        public string TranslatorName { get; set; }
        public DateTime CreatedDateUtc { get; set; }

        public List<PlaylistMovieVideoQualityDto> PlaylistMovieVideoQualityDtos { get; set; }
    }
}
