using Kozachok.Shared.Abstractions.Commands;
using Microsoft.AspNetCore.Http;

namespace Kozachok.Domain.Commands.File
{
    public class UpdateUserThumbnailImageCommand : RequestCommand<Shared.DTO.Models.File>
    {
        public IFormFile File { get; set; }
    }
}
