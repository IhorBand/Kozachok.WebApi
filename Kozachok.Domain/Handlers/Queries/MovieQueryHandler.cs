using Kozachok.Domain.Handlers.Common;
using Kozachok.Shared;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Models.DbEntities;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using Kozachok.Domain.Queries.Movie;

namespace Kozachok.Domain.Handlers.Queries
{
    public class MovieQueryHandler :
        QueryHandler,
        IRequestHandler<GetMovieQuery, PagedResult<Movie>>
    {
        private readonly IMovieRepository movieRepository;

        public MovieQueryHandler(
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,
            IMovieRepository movieRepository
        )
        : base(
                bus,
                notifications
        )
        {
            this.movieRepository = movieRepository;
        }

        public async Task<PagedResult<Movie>> Handle(GetMovieQuery request, CancellationToken cancellationToken)
        {
            var currentPage = 1;
            var itemsPerPage = GlobalConstants.DefaultPageSize;

            if (request.Page != null && request.Page > 0)
            {
                currentPage = request.Page.Value;
            }

            if (request.ItemsPerPage != null && request.ItemsPerPage > 0 && request.ItemsPerPage < itemsPerPage)
            {
                itemsPerPage = request.ItemsPerPage.Value;
            }

            var result = await movieRepository.GetMoviesAsync(
                request.MovieTypeId, 
                request.MovieMainCategoryId, 
                request.SearchValue.ToLower(), 
                currentPage, 
                itemsPerPage, 
                request.GenreId ?? 0, 
                request.CountryId ?? 0,
                request.MovieOrderTypeId,
                request.OrderDirection);

            return result;
        }
    }
}
