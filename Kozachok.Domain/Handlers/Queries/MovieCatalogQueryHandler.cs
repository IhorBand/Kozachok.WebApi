using Kozachok.Domain.Handlers.Common;
using Kozachok.Domain.Queries.MovieCatalog;
using Kozachok.Shared;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.Abstractions.HttpRepositories;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Models.HttpRepository.MovieCatalog;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kozachok.Domain.Handlers.Queries
{
    public class MovieCatalogQueryHandler :
        QueryHandler,
        IRequestHandler<GetMovieTranslatorsQuery, List<Translator>>,
        IRequestHandler<GetMovieSeasonsQuery, GetSeasonResult>,
        IRequestHandler<GetMovieStreamQuery, Stream>
    {
        private readonly IMovieCatalogRepository movieCatalogRepository;
        private readonly IMovieRepository movieRepository;

        public MovieCatalogQueryHandler(
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,
            IMovieRepository movieRepository,
            IMovieCatalogRepository movieCatalogRepository
        )
        : base(
                bus,
                notifications
        )
        {
            this.movieCatalogRepository = movieCatalogRepository;
            this.movieRepository = movieRepository;
        }

        public async Task<List<Translator>> Handle(GetMovieTranslatorsQuery request, CancellationToken cancellationToken)
        {
            var movie = await movieRepository.FirstOrDefaultAsync(m => m.Id == request.MovieId);
            
            if (movie == null)
            {
                await Bus.InvokeDomainNotificationAsync("Movie not found.");
                return null;
            }
            
            var result = await movieCatalogRepository.GetTranslatorsAsync(movie.VideoUrlId);

            return result;
        }

        public async Task<GetSeasonResult> Handle(GetMovieSeasonsQuery request, CancellationToken cancellationToken)
        {
            var movie = await movieRepository.FirstOrDefaultAsync(m => m.Id == request.MovieId);

            if (movie == null)
            {
                await Bus.InvokeDomainNotificationAsync("Movie not found.");
                return null;
            }

            var result = await movieCatalogRepository.GetSeasonsAsync(movie.VideoId, request.TranslatorId);

            return result;
        }

        public async Task<Stream> Handle(GetMovieStreamQuery request, CancellationToken cancellationToken)
        {
            var movie = await movieRepository.FirstOrDefaultAsync(m => m.Id == request.MovieId);

            if (movie == null)
            {
                await Bus.InvokeDomainNotificationAsync("Movie not found.");
                return null;
            }

            var movieType = GlobalConstants.MovieTypeFilm;

            if (movie.TypeId == Shared.DTO.Enums.MovieType.Series)
            {
                movieType = GlobalConstants.MovieTypeSeries;
            }

            var result = await movieCatalogRepository.GetMovieStreamAsync(movie.VideoId, request.TranslatorId, movieType, request.Season, request.Episode);

            return result;
        }
    }
}
