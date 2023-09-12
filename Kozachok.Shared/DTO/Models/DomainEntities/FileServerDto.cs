using System;

namespace Kozachok.Shared.DTO.Models.DomainEntities
{
    public class FileServerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDateUtc { get; set; }
    }
}
