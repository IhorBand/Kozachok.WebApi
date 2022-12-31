using Kozachok.Shared.Abstractions.Commands;
using Microsoft.AspNetCore.Http;

namespace Kozachok.Domain.Commands.User
{
    public class UpdateUserThumbnailImageCommand : RequestCommand<Shared.DTO.Models.File>
    {
        public IFormFile File { get; set; }
    }
}
