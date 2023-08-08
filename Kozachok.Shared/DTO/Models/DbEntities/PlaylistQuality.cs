using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Enums;
using System;

namespace Kozachok.Shared.DTO.Models.DbEntities
{
    public class PlaylistQuality : Entity
    {
        // -> Empty contructor for EF
        public PlaylistQuality()
        {
            CreatedDateUTC = DateTime.UtcNow;
        }

        public virtual Guid PlaylistMovieId { get; set; }
        public virtual VideoQualityType QualityId { get; set; }
        public virtual string MovieUrl { get; set; }
        public virtual DateTime CreatedDateUTC { get; private set; }
    }
}
