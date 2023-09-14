using Kozachok.Shared.Abstractions.Repositories.Common;
using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Enums;
using Kozachok.Shared.DTO.Models.DbEntities;
using System.Threading;
using System.Threading.Tasks;

namespace Kozachok.Shared.Abstractions.Repositories
{
    public interface IMovieRepository : ICrudRepository<Movie>
    {
        public Task<PagedResult<Movie>> GetMoviesAsync(
            MovieType? movieType,
            MovieMainCategory? mainCategory,
            string searchValue = "",
            int pageIndex = 1,
            int pageSize = GlobalConstants.DefaultPageSize,
            int genreId = 0,
            int countryId = 0,
            MovieOrderType orderType = MovieOrderType.CreatedDate,
            OrderDirection orderDirection = OrderDirection.Descending,
            CancellationToken cancellationToken = default);
    }
}
