using AutoMapper;
using Kozachok.Domain.Commands.File;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.DTO.Common;
using Kozachok.WebApi.Controllers.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kozachok.WebApi.Controllers
{
    public class FileController : UserControllerBase
    {
        public FileController(
            IMediatorHandler mediator,
            IMapper mapper,
            INotificationHandler<DomainNotification> notifications
            ) : base(mediator, mapper, notifications)
        {
        }

        [HttpPost("UploadUserThumbnailImage")]
        public async Task<IActionResult> UploadUserThumbnailImage(IFormFile file)
        {
            var result = await bus.RequestAsync(new UploadUserThumbnailImageFileCommand() { File = file });
            
            if (result != null)
            {
                var model = mapper.Map<Models.File.File>(result);
                return Response(model);
            }

            return Response();
        }
    }
}
