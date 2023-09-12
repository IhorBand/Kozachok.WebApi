using Kozachok.Domain.Handlers.Common;
using Kozachok.Shared;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Common;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using AutoMapper;
using Kozachok.Domain.Queries.Movie;
using Kozachok.Shared.DTO.Models.DomainEntities;

namespace Kozachok.Domain.Handlers.Queries
{
    public class MovieQueryHandler :
        QueryHandler,
        IRequestHandler<GetMovieQuery, PagedResult<MovieDto>>
    {
        private readonly IMovieRepository movieRepository;
        private readonly IMapper mapper;

        public MovieQueryHandler(
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,
            IMovieRepository movieRepository, 
            IMapper mapper)
        : base(
                bus,
                notifications
        )
        {
            this.movieRepository = movieRepository;
            this.mapper = mapper;
        }

        public async Task<PagedResult<MovieDto>> Handle(GetMovieQuery request, CancellationToken cancellationToken)
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
            
            return mapper.Map<PagedResult<MovieDto>>(result);
        }
    }
}
