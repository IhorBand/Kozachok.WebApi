using System.Collections.Generic;

namespace Kozachok.Shared.DTO.Models.HttpRepository.MovieCatalog
{
    public class Stream
    {
        public string Episode { get; set; }
        public List<Resolution> Resolutions { get; set; }
        public string Season { get; set; }
    }
}
