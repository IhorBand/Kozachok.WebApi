using System;
using System.Collections.Generic;
using System.Text;

namespace Kozachok.Shared.DTO.Models.HttpRepository.MovieCatalog
{
    public class Stream
    {
        public string episode { get; set; }
        public List<Resolution> resolutions { get; set; }
        public string season { get; set; }
    }
}
