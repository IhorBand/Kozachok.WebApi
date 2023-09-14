using Kozachok.Shared.DTO.Models.HttpRepository.MovieCatalog;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kozachok.Shared.Abstractions.HttpRepositories
{
    public interface IMovieCatalogRepository
    {
        public Task<List<Translator>> GetTranslatorsAsync(string videoUrlId, CancellationToken cancellationToken = default);
        public Task<GetSeasonResult> GetSeasonsAsync(string filmId, string translatorId, CancellationToken cancellationToken = default);
        public Task<Stream> GetMovieStreamAsync(string filmId, string translatorId, string filmType, int season, int episode, CancellationToken cancellationToken = default);
    }
}
