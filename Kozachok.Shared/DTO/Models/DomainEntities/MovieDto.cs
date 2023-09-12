using Kozachok.Shared.DTO.Enums;
using System;

namespace Kozachok.Shared.DTO.Models.DomainEntities
{
    public class MovieDto
    {
        public Guid Id { get; set; }
        public string AdditionalCoverUrl { get; set; }
        public string ShortTitle { get; set; }
        public string FullTitle { get; set; }
        public string OriginalFullTitle { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string ImdbId { get; set; }
        public string ImdbUrl { get; set; }
        public string ImdbCoverUrl { get; set; }
        public string ImdbDescription { get; set; }
        public string VideoFullUrl { get; set; }
        public MovieType TypeId { get; set; }
        public MovieMainCategory MainCategoryId { get; set; }
        public DateTime CreatedDateUtc { get; set; }
    }
}
