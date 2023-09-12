using Kozachok.Shared.Abstractions.Commands;
using Kozachok.Shared.DTO.Models.DomainEntities;
using Microsoft.AspNetCore.Http;

namespace Kozachok.Domain.Commands.File
{
    public class UpdateUserThumbnailImageCommand : RequestCommand<FileDto>
    {
        public IFormFile File { get; set; }
    }
}
