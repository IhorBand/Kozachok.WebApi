using System;
using System.Collections.Generic;

namespace Kozachok.Shared.DTO.Models.DomainEntities
{
    public class PlaylistMovieDto
    {
        public Guid Id { get; set; }
        public MovieDto Movie { get; set; }
        public int OrderNumber { get; set; }
        public string Name { get; set; }
        public List<PlaylistMovieVideoDto> PlaylistMovieVideoDtOs { get; set; }
    }
}
