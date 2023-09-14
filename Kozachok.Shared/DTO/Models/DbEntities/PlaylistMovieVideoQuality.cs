using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Enums;
using System;

namespace Kozachok.Shared.DTO.Models.DbEntities
{
    public class PlaylistMovieVideoQuality : Entity
    {
        public PlaylistMovieVideoQuality()
        {
            CreatedDateUtc = DateTime.UtcNow;
        }

        public virtual Guid PlaylistMovieId { get; set; }
        public virtual Guid PlaylistMovieVideoId { get; set; }
        public virtual VideoQualityType QualityId { get; set; }
        public virtual string MovieUrl { get; set; }
        public virtual DateTime CreatedDateUtc { get; private set; }

        public virtual PlaylistMovie PlaylistMovie { get; set; }
        public virtual PlaylistMovieVideo PlaylistMovieVideo { get; set; }
    }
}
