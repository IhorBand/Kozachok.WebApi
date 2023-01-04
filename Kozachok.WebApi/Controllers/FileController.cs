﻿using AutoMapper;
using Kozachok.Domain.Commands.File;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.DTO.Common;
using Kozachok.WebApi.Auth;
using Kozachok.WebApi.Controllers.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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

        [HttpPost("UpdateUserThumbnailImage")]
        [BearerAuthorization]
        [RequestSizeLimit(2097152)] // Allow to upload only 2MB
        [RequestFormLimits(MultipartBodyLengthLimit = 2097152, ValueLengthLimit = 2097152)]
        public async Task<IActionResult> UpdateUserThumbnailImage([FileExtensions(Extensions = "jpg,png,gif,jpeg,bmp,svg")] IFormFile file)
        {
            var result = await bus.RequestAsync(new UpdateUserThumbnailImageCommand() { File = file });
            
            if (result != null)
            {
                var model = mapper.Map<Models.File.File>(result);
                return Response(model);
            }

            return Response();
        }
    }
}
