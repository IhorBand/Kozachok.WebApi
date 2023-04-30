using Kozachok.Shared.Abstractions.Commands;
using Kozachok.Shared.DTO.Models.HttpRepository.MovieCatalog;
using System;

namespace Kozachok.Domain.Queries.MovieCatalog
{
    public class GetMovieSeasonsQuery : RequestCommand<GetSeasonResult>
    {
        public Guid MovieId { get; set; }
        public string TranslatorId { get; set; }
    }
}
