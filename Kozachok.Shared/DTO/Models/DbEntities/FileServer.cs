using Kozachok.Shared.DTO.Common;
using System;
using System.Collections.Generic;

namespace Kozachok.Shared.DTO.Models.DbEntities
{

    public class FileServer : Entity
    {
        public virtual string Name { get; private set; }
        public virtual string Path { get; private set; }
        public virtual string Url { get; private set; }
        public virtual bool IsActive { get; private set; }
        public virtual ICollection<File> Files { get; set; }
        public virtual DateTime CreatedDateUtc { get; private set; }

        public static FileServer Create(string name, string path, string url, bool isActive)
        {
            return new FileServer
            {
                Name = name,
                Path = path,
                Url = url,
                IsActive = isActive,
                CreatedDateUtc = DateTime.UtcNow
            };
        }
    }
}
