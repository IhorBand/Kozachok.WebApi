using System.Threading;
using Kozachok.Repository.Contexts;
using Kozachok.Repository.Repositories.Common;
using Kozachok.Shared;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Enums;
using Kozachok.Shared.DTO.Models.DbEntities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Kozachok.Repository.Repositories
{
    public class MovieRepository : CrudRepository<Movie>, IMovieRepository
    {
        public MovieRepository(MainDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<Movie>> GetMoviesAsync(
            MovieType? movieType,
            MovieMainCategory? mainCategory,
            string searchValue = "", 
            int pageIndex = 1, 
            int pageSize = GlobalConstants.DefaultPageSize, 
            int genreId = 0, 
            int countryId = 0,
            MovieOrderType orderType = MovieOrderType.CreatedDate,
            OrderDirection orderDirection = OrderDirection.Descending,
            CancellationToken cancellationToken = default)
        {
            const string sql = "EXEC sp_SearchMovies @SearchValue, @PageIndex, @PageSize, @GenreId, @CountryId, @MovieTypeId, @MovieMainCategoryId, @OrderId, @OrderDirectionId, @TotalCount OUTPUT, @TotalPages OUTPUT";

            var movieTypeId = movieType == null ? -1 : (int)movieType;
            var movieMainCategoryId = mainCategory == null ? 0 : (int)mainCategory;

            var totalCountParameter = new SqlParameter { ParameterName = "@TotalCount", Direction = System.Data.ParameterDirection.Output, SqlDbType = System.Data.SqlDbType.Int };
            var totalPagesParameter = new SqlParameter { ParameterName = "@TotalPages", Direction = System.Data.ParameterDirection.Output, SqlDbType = System.Data.SqlDbType.Int };

            var parameters = new []
            {
                new SqlParameter { ParameterName = "@SearchValue", Value = searchValue, Direction = System.Data.ParameterDirection.Input, SqlDbType = System.Data.SqlDbType.NVarChar },
                new SqlParameter { ParameterName = "@PageIndex", Value = pageIndex, Direction = System.Data.ParameterDirection.Input, SqlDbType = System.Data.SqlDbType.Int },
                new SqlParameter { ParameterName = "@PageSize", Value = pageSize, Direction = System.Data.ParameterDirection.Input, SqlDbType = System.Data.SqlDbType.Int },
                new SqlParameter { ParameterName = "@GenreId", Value = genreId, Direction = System.Data.ParameterDirection.Input, SqlDbType = System.Data.SqlDbType.Int },
                new SqlParameter { ParameterName = "@CountryId", Value = countryId, Direction = System.Data.ParameterDirection.Input, SqlDbType = System.Data.SqlDbType.Int },
                new SqlParameter { ParameterName = "@MovieTypeId", Value = movieTypeId, Direction = System.Data.ParameterDirection.Input, SqlDbType = System.Data.SqlDbType.Int },
                new SqlParameter { ParameterName = "@MovieMainCategoryId", Value = movieMainCategoryId, Direction = System.Data.ParameterDirection.Input, SqlDbType = System.Data.SqlDbType.Int },
                new SqlParameter { ParameterName = "@OrderId", Value = (int)orderType, Direction = System.Data.ParameterDirection.Input, SqlDbType = System.Data.SqlDbType.Int },
                new SqlParameter { ParameterName = "@OrderDirectionId", Value = (int)orderDirection, Direction = System.Data.ParameterDirection.Input, SqlDbType = System.Data.SqlDbType.Int },

                totalCountParameter,
                totalPagesParameter
            };
            
            var result = await Context.Set<Movie>().FromSqlRaw(sql, parameters).ToListAsync(cancellationToken);

            var model = new PagedResult<Movie>(pageIndex, (int)totalPagesParameter.Value, (int)totalCountParameter.Value, pageSize, result);

            return model; 
        }
    }
}
