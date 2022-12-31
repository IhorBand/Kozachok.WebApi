using Kozachok.Domain.Events.User;
using Kozachok.Domain.Handlers.Common;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.Abstractions.Repositories.Common;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Models;
using MediatR;
using System;
using System.Threading.Tasks;
using System.Threading;
using Kozachok.Utils.Validation;
using Kozachok.Shared.DTO.Configuration;
using Kozachok.Domain.Commands.File;

namespace Kozachok.Domain.Handlers.Commands
{
    public class FileCommandHandler :
        CommandHandler,
        IRequestHandler<UploadUserThumbnailImageFileCommand, File>
    {
        private readonly IFileServerRepository fileServerRepository;
        private readonly IFileRepository fileRepository;

        private readonly FileServerConfiguration fileServerConfiguration;

        public FileCommandHandler(
            IUnitOfWork uow,
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,
            IFileServerRepository fileServerRepository,
            IFileRepository fileRepository,
            FileServerConfiguration fileServerConfiguration
        )
        : base(
                uow,
                bus,
                notifications
        )
        {
            this.fileServerRepository = fileServerRepository;
            this.fileRepository = fileRepository;
            this.fileServerConfiguration = fileServerConfiguration;
        }

        public async Task<File> Handle(UploadUserThumbnailImageFileCommand request, CancellationToken cancellationToken)
        {
            request
                .Is(e => e.File.Length <= 0, async () => await bus.InvokeDomainNotificationAsync("File wasn't uploaded."));

            if (!IsValidOperation())
            {
                return null;
            }

            var fileServer = await fileServerRepository.FirstOrDefaultAsync(fs => fs.IsActive);

            var filePath = fileServer.Path + fileServerConfiguration.UserAvatarPath;
            var extension = System.IO.Path.GetExtension(request.File.FileName);
            var fileName = Guid.NewGuid().ToString().Replace("-", "") + extension;
            var fullFilePath = System.IO.Path.Combine(filePath, fileName);
            var fileUrl = fileServer.Url + fileServerConfiguration.UserAvatarPath + fileName;

            var uploadedFile = new File(fileName, fileServer.Id, Shared.DTO.Enums.FileType.Image, extension, fullFilePath, fileUrl, false);
            uploadedFile.SetSize(request.File.Length);

            await fileRepository.AddAsync(uploadedFile);

            using (var fileStream = new System.IO.FileStream(fullFilePath, System.IO.FileMode.Create))
            {
                await request.File.CopyToAsync(fileStream);
            }

            Commit();

            return uploadedFile;
        }
    }
}
