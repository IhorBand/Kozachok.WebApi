using Kozachok.Shared.Abstractions.Commands;
using Kozachok.Shared.DTO.Models.HttpRepository.MovieCatalog;
using System;

namespace Kozachok.Domain.Queries.MovieCatalog
{
    public class GetMovieStreamQuery : RequestCommand<Stream>
    {
        public Guid MovieId { get; set; }
        public string TranslatorId { get; set; }
        public int Season { get; set; }
        public int Episode { get; set; }
    }
}
