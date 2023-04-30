using AutoMapper;
using Kozachok.Domain.Queries.Movie;
using Kozachok.Domain.Queries.MovieCatalog;
using Kozachok.Shared;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Enums;
using Kozachok.Shared.DTO.Models.HttpRepository.MovieCatalog;
using Kozachok.WebApi.Auth;
using Kozachok.WebApi.Controllers.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kozachok.WebApi.Controllers
{
    public class MovieCatalogController : UserControllerBase
    {
        public MovieCatalogController(
            IMediatorHandler mediator,
            IMapper mapper,
            INotificationHandler<DomainNotification> notifications
            ) : base(mediator, mapper, notifications)
        {
        }

        [HttpGet("{MovieId}/Translators")]
        [BearerAuthorization]
        public async Task<IActionResult> GetMovieTranslator(
            [FromRoute(Name = "MovieId")] Guid movieId)
        {
            var result = await bus.RequestAsync(new GetMovieTranslatorsQuery()
            {
                MovieId = movieId
            });
            return Response(result);
        }

        [HttpGet("{MovieId}/Translator/{TranslatorId}/Seasons")]
        [BearerAuthorization]
        public async Task<IActionResult> GetMovieSeasons(
            [FromRoute(Name = "MovieId")] Guid movieId,
            [FromRoute(Name = "TranslatorId")] string translatorId)
        {
            var result = await bus.RequestAsync(new GetMovieSeasonsQuery()
            {
                MovieId = movieId,
                TranslatorId = translatorId
            });
            return Response(result);
        }

        [HttpGet("{MovieId}/Translator/{TranslatorId}/Season/{season}/Episode/{episode}")]
        [BearerAuthorization]
        public async Task<IActionResult> GetMovieSeasons(
            [FromRoute(Name = "MovieId")] Guid movieId,
            [FromRoute(Name = "TranslatorId")] string translatorId,
            [FromRoute(Name = "season")] int season = 1,
            [FromRoute(Name = "episode")] int episode = 1)
        {
            var result = await bus.RequestAsync(new GetMovieStreamQuery()
            {
                MovieId = movieId,
                TranslatorId = translatorId,
                Season = season,
                Episode = episode
            });
            return Response(result);
        }
    }
}
