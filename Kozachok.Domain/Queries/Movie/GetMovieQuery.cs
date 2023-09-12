using Kozachok.Shared.Abstractions.Commands;
using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Enums;
using Kozachok.Shared.DTO.Models.DomainEntities;

namespace Kozachok.Domain.Queries.Movie
{
    public class GetMovieQuery : RequestCommand<PagedResult<MovieDto>>
    {
        public int? Page { get; set; }
        public int? ItemsPerPage { get; set; }
        public string SearchValue { get; set; }
        public int? GenreId { get; set; }
        public int? CountryId { get; set; }
        public MovieType? MovieTypeId { get; set; }
        public MovieMainCategory? MovieMainCategoryId { get; set; }
        public MovieOrderType MovieOrderTypeId { get; set; }
        public OrderDirection OrderDirection { get; set; }
    }
}
