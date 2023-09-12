using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Enums;
using System;
using System.Collections.Generic;

namespace Kozachok.Shared.DTO.Models.DbEntities
{
    public class File : Entity
    {
        public virtual string Name { get; private set; }
        public virtual Guid FileServerId { get; private set; }
        public virtual FileType FileTypeId { get; private set; }
        public virtual long? Size { get; private set; }
        public virtual string Extension { get; private set; }
        public virtual string FullPath { get; private set; }
        public virtual string Url { get; private set; }
        public virtual bool IsAcknowledged { get; private set; }
        public virtual DateTime CreatedDateUtc { get; private set; }

        public virtual FileServer FileServer { get; set; }
        public virtual ICollection<User> Users { get; set; }

        public void SetSize(long size) => Size = size;

        public static File Create(
            string name,
            Guid fileServerId,
            FileType fileTypeId,
            string extension,
            string fullPath,
            string url,
            bool isAcknowledged)
        {
            return new File
            {
                Name = name,
                FileServerId = fileServerId,
                FileTypeId = fileTypeId,
                Extension = extension,
                FullPath = fullPath,
                Url = url,
                IsAcknowledged = isAcknowledged,
                CreatedDateUtc = DateTime.UtcNow
            };
        }
    }
}
