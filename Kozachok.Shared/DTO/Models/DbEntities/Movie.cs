using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Enums;
using System;

namespace Kozachok.Shared.DTO.Models.DbEntities
{
    public class Movie : Entity
    {
        // -> Empty contructor for EF
        public Movie()
        {
            
        }

        public virtual string VideoId { get; set; }
        public virtual string VideoUrlId { get; set; }
        public virtual string AdditionalCoverUrl { get; set; }
        public virtual string ShortTitle { get; set; }
        public virtual string FullTitle { get; set; }
        public virtual string OriginalFullTitle { get; set; }
        public virtual string ShortDescription { get; set; }
        public virtual string FullDescription { get; set; }
        public virtual string ImdbId { get; set; }
        public virtual string ImdbUrl { get; set; }
        public virtual string ImdbCoverUrl { get; set; }
        public virtual string ImdbDescription { get; set; }
        public virtual string VideoFullUrl { get; set; }
        public virtual MovieType TypeId { get; set; }
        public virtual MovieMainCategory MainCategoryId { get; set; }
        public virtual DateTime ReleaseDate { get; set; }
        public virtual DateTime CreatedDateUTC { get; private set; }
    }
}
