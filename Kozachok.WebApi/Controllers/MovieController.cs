using AutoMapper;
using Kozachok.Domain.Queries.Movie;
using Kozachok.Shared;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Enums;
using Kozachok.WebApi.Auth;
using Kozachok.WebApi.Controllers.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kozachok.WebApi.Controllers
{
    public class MovieController : UserControllerBase
    {
        public MovieController(
            IMediatorHandler mediator,
            IMapper mapper,
            INotificationHandler<DomainNotification> notifications
            ) : base(mediator, mapper, notifications)
        {
        }

        [HttpGet("Search")]
        [BearerAuthorization]
        public async Task<IActionResult> GetMovies(
            [FromQuery(Name = "movieTypeId")] MovieType? movieTypeId,
            [FromQuery(Name = "movieMainCategoryId")] MovieMainCategory? movieMainCategoryId,
            [FromQuery(Name = "page")] int? page = 1, 
            [FromQuery(Name = "itemsPerPage")] int? itemsPerPage = GlobalConstants.DefaultPageSize, 
            [FromQuery(Name = "name")] string? name = "",
            [FromQuery(Name = "genreId")] int? genreId = 0,
            [FromQuery(Name = "countryId")] int? countryId = 0,
            [FromQuery(Name = "orderId")] MovieOrderType orderId = MovieOrderType.CreatedDate,
            [FromQuery(Name = "orderDirectionId")] OrderDirection orderDirectionId = OrderDirection.Descending)
        {
            var result = await bus.RequestAsync(new GetMovieQuery() 
            { 
                Page = page, 
                ItemsPerPage = itemsPerPage,
                SearchValue = name,
                GenreId = genreId,
                CountryId = countryId,
                MovieTypeId = movieTypeId,
                MovieMainCategoryId = movieMainCategoryId,
                MovieOrderTypeId = orderId,
                OrderDirection = orderDirectionId
            });
            return Response<PagedResult<Models.Movie.Movie>>(result);
        }
    }
}
