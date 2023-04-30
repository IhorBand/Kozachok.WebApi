using Kozachok.Shared.Abstractions.Commands;
using Kozachok.Shared.DTO.Models.HttpRepository.MovieCatalog;
using System;
using System.Collections.Generic;

namespace Kozachok.Domain.Queries.MovieCatalog
{
    public class GetMovieTranslatorsQuery : RequestCommand<List<Translator>>
    {
        public Guid MovieId { get; set; }
    }
}
