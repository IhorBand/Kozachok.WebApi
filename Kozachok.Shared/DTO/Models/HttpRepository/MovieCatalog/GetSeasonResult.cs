using System.Collections.Generic;

namespace Kozachok.Shared.DTO.Models.HttpRepository.MovieCatalog
{
    public class GetSeasonResult
    {
        public List<Episode> Episodes { get; set; }
        public List<Season> Seasons { get; set; }
        public string TranslatorId { get; set; }
    }
}
