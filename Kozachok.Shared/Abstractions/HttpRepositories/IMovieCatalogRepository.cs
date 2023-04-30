using Kozachok.Shared.DTO.Models.HttpRepository.MovieCatalog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kozachok.Shared.Abstractions.HttpRepositories
{
    public interface IMovieCatalogRepository
    {
        public Task<List<Translator>> GetTranslatorsAsync(string videoUrlId);
        public Task<GetSeasonResult> GetSeasonsAsync(string filmId, string translatorId);
        public Task<Stream> GetMovieStreamAsync(string filmId, string translatorId, string filmType, int season, int episode);
    }
}
