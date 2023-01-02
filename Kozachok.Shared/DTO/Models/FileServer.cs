using Kozachok.Shared.DTO.Common;
using System;

namespace Kozachok.Shared.DTO.Models
{

    public class FileServer : Entity
    {
        // -> Empty contructor for EF
        public FileServer()
        {

        }

        public FileServer(string name, string path, string url, bool isActive)
        {
            Name = name;
            Path = path;
            Url = url;
            IsActive = isActive;
            CreatedDateUTC = DateTime.UtcNow;
        }

        public virtual string Name { get; private set; }
        public virtual string Path { get; private set; }
        public virtual string Url { get; private set; }
        public virtual bool IsActive { get; private set; }
        public virtual DateTime CreatedDateUTC { get; private set; }
    }
}
