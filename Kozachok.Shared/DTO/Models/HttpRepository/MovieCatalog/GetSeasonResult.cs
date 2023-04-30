using System.Collections.Generic;

namespace Kozachok.Shared.DTO.Models.HttpRepository.MovieCatalog
{
    public class GetSeasonResult
    {
        public List<Episode> episodes { get; set; }
        public List<Season> seasons { get; set; }
        public string translator_id { get; set; }
    }
}
