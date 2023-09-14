using Kozachok.Shared.DTO.Common;
using System;
using System.Collections.Generic;

namespace Kozachok.Shared.DTO.Models.DbEntities
{
    public class PlaylistMovieVideo : Entity
    {
        public PlaylistMovieVideo()
        {
            CreatedDateUtc = DateTime.UtcNow;
        }

        public virtual Guid PlaylistMovieId { get; set; }
        public virtual int Season { get; set; }
        public virtual int Episode { get; set; }
        public virtual string TranslatorExternalId { get; set; }
        public virtual string TranslatorName { get; set; }
        public virtual DateTime CreatedDateUtc { get; private set; }

        public virtual PlaylistMovie PlaylistMovie { get; set; }
        public virtual ICollection<PlaylistMovieVideoQuality> PlaylistMovieVideoQualities { get; set; }
    }
}
