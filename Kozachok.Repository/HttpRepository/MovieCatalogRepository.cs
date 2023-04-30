using Kozachok.Shared.Abstractions.HttpRepositories;
using Kozachok.Shared.DTO.Configuration;
using Kozachok.Shared.DTO.Models.HttpRepository.MovieCatalog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kozachok.Repository.HttpRepository
{
    public class MovieCatalogRepository : IMovieCatalogRepository
    {
        private MovieCatalogEndpointsConfiguration MovieCatalogEndpoints { get; set; }

        public MovieCatalogRepository(
            MovieCatalogEndpointsConfiguration movieCatalogEndpoints)
        {
            MovieCatalogEndpoints = movieCatalogEndpoints;
        }

        public async Task<List<Translator>> GetTranslatorsAsync(string videoUrlId)
        {
            var url = MovieCatalogEndpoints.BaseUrl + "movie/translators";
            var parameters = $"?filmUrlId={Uri.EscapeDataString(videoUrlId)}";

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);

            HttpResponseMessage response = await client.GetAsync(parameters).ConfigureAwait(false);

            if(response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<Translator>>(jsonString);
                return result;
            }

            return null;
        }

        public async Task<GetSeasonResult> GetSeasonsAsync(string filmId, string translatorId)
        {
            var url = MovieCatalogEndpoints.BaseUrl + "movie/seasons";
            var parameters = $"?filmId={Uri.EscapeDataString(filmId)}&translatorId={Uri.EscapeDataString(translatorId)}";

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);

            HttpResponseMessage response = await client.GetAsync(parameters).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<GetSeasonResult>(jsonString);
                return result;
            }

            return null;
        }

        public async Task<Stream> GetMovieStreamAsync(string filmId, string translatorId, string filmType, int season, int episode)
        {
            var url = MovieCatalogEndpoints.BaseUrl + "movie/stream";
            var parameters = $"?filmId={Uri.EscapeDataString(filmId)}&translatorId={Uri.EscapeDataString(translatorId)}&filmType={Uri.EscapeDataString(filmType)}&season={Uri.EscapeDataString(season.ToString())}&episode={Uri.EscapeDataString(episode.ToString())}";

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);

            HttpResponseMessage response = await client.GetAsync(parameters).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Stream>(jsonString);
                return result;
            }

            return null;
        }
    }
}
