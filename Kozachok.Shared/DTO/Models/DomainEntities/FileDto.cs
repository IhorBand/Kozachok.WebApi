using Kozachok.Shared.DTO.Enums;
using System;

namespace Kozachok.Shared.DTO.Models.DomainEntities
{
    public class FileDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid FileServerId { get; set; }
        public FileType FileTypeId { get; set; }
        public long? Size { get; set; }
        public string Extension { get; set; }
        public string FullPath { get; set; }
        public string Url { get; set; }
        public bool IsAcknowledged { get; set; }
        public DateTime CreatedDateUtc { get; set; }

        public FileServerDto FileServerDto { get; set; }
    }
}
