using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Enums;
using System;

namespace Kozachok.Shared.DTO.Models
{
    public class File : Entity
    {
        // -> Empty contructor for EF
        public File()
        {

        }

        public File(
            string name, 
            Guid fileServerId, 
            FileType fileTypeId,
            string extension,
            string fullPath,
            string url,
            bool isAcknowledged)
        {
            Name = name;
            FileServerId = fileServerId;
            FileTypeId = fileTypeId;
            Extension = extension;
            FullPath = fullPath;
            Url = url;
            IsAcknowledged = isAcknowledged;
            CreatedDateUTC = DateTime.UtcNow;
        }

        public virtual string Name { get; private set; }
        public virtual Guid FileServerId { get; private set; }
        public virtual FileType FileTypeId { get; private set; }
        public virtual long? Size { get; private set; }
        public virtual string Extension { get; private set; }
        public virtual string FullPath { get; private set; }
        public virtual string Url { get; private set; }
        public virtual bool IsAcknowledged { get; private set; }
        public virtual DateTime CreatedDateUTC { get; private set; }

        public void SetSize(long size) => Size = size;
    }
}
