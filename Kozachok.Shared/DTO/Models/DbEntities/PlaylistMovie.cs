using Kozachok.Shared.DTO.Common;
using System;

namespace Kozachok.Shared.DTO.Models.DbEntities
{
    public class PlaylistMovie : Entity
    {
        // -> Empty contructor for EF
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
    }
}
