﻿using Kozachok.Shared.DTO.Common;
using System;
using System.Collections.Generic;

namespace Kozachok.Shared.DTO.Models.DbEntities
{
    public class PlaylistMovie : Entity
    {
        public PlaylistMovie()
        {
            CreatedDateUtc = DateTime.UtcNow;
        }

        public virtual Guid MovieId { get; set; }
        public virtual Guid RoomId { get; set; }
        public virtual int OrderNumber { get; set; }
        public virtual string VideoId { get; set; }
        public virtual string VideoUrlId { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime CreatedDateUtc { get; private set; }

        public virtual Movie Movie { get; set; }
        public virtual Room Room { get; set; }
        public virtual ICollection<PlaylistMovieVideo> PlaylistMovieVideos { get; set; }
    }
}
