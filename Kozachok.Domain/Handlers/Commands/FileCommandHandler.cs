﻿using Kozachok.Domain.Handlers.Common;
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
using Kozachok.Shared.Abstractions.Identity;
using Kozachok.Domain.Commands.File;
using Microsoft.Extensions.Logging;

namespace Kozachok.Domain.Handlers.Commands
{
    public class FileCommandHandler :
        CommandHandler,
        IRequestHandler<UpdateUserThumbnailImageCommand, File>
    {
        private readonly IFileServerRepository fileServerRepository;
        private readonly IFileRepository fileRepository;
        private readonly IUserRepository userRepository;

        private readonly FileServerConfiguration fileServerConfiguration;
        private readonly IUser user;
        private readonly ILogger<FileCommandHandler> logger;

        public FileCommandHandler(
            IUnitOfWork uow,
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,
            IFileServerRepository fileServerRepository,
            IFileRepository fileRepository,
            IUserRepository userRepository,
            FileServerConfiguration fileServerConfiguration,
            IUser user,
            ILogger<FileCommandHandler> logger
        )
        : base(
                uow,
                bus,
                notifications
        )
        {
            this.fileServerRepository = fileServerRepository;
            this.fileRepository = fileRepository;
            this.userRepository = userRepository;
            this.fileServerConfiguration = fileServerConfiguration;
            this.user = user;
            this.logger = logger;
        }

        public async Task<File> Handle(UpdateUserThumbnailImageCommand request, CancellationToken cancellationToken)
        {
            request
                .Is(e => e.File.Length <= 0, async () => await bus.InvokeDomainNotificationAsync("File wasn't uploaded."));

            if (!IsValidOperation())
            {
                return null;
            }

            if (user.Id == null)
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return null;
            }

            var currentUser = await userRepository.GetAsync(user.Id.Value);

            if (currentUser == null)
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return null;
            }

            var fileServer = await fileServerRepository.FirstOrDefaultAsync(fs => fs.IsActive);

            if (fileServer == null || (fileServer != null && fileServer.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("No available file server at the moment.");
                return null;
            }

            var filePath = fileServer.Path + fileServerConfiguration.UserAvatarPath;
            var extension = System.IO.Path.GetExtension(request.File.FileName);
            var fileName = Guid.NewGuid().ToString().Replace("-", "") + extension;
            var fullFilePath = System.IO.Path.Combine(filePath, fileName);
            var fileUrl = fileServer.Url + fileServerConfiguration.UserAvatarPath + fileName;

            var uploadedFile = new File(fileName, fileServer.Id, Shared.DTO.Enums.FileType.Image, extension, fullFilePath, fileUrl, true);
            uploadedFile.SetSize(request.File.Length);

            await fileRepository.AddAsync(uploadedFile);

            var oldImageFileId = currentUser.ThumbnailImageFileId;

            currentUser.SetThumbnailImageFileId(uploadedFile.Id);
            await userRepository.UpdateAsync(currentUser);
            
            try
            { 
                Commit();

                File previousImage = null;
                if (oldImageFileId != null)
                {
                    previousImage = await fileRepository.GetAsync(oldImageFileId.Value);
                    if (previousImage != null)
                    {
                        await fileRepository.DeleteAsync(previousImage.Id);
                    }
                }

                try
                {
                    Commit();

                    using (var fileStream = new System.IO.FileStream(fullFilePath, System.IO.FileMode.Create))
                    {
                        await request.File.CopyToAsync(fileStream);
                    }

                    if (previousImage != null)
                    {
                        System.IO.File.Delete(previousImage.FullPath);
                    }

                }
                catch (Exception ex)
                {
                    await bus.InvokeDomainNotificationAsync("Cannot Update User's Thumbnail Image.");
                    var userIdExStr = user != null ? user.Id.ToString() : "Unauthorized User";
                    logger.LogError(ex, $"Cannot Update User's Thumbnail Image. UserId: {userIdExStr}");
                    throw;
                }
            }
            catch(Exception ex)
            {
                await bus.InvokeDomainNotificationAsync("Cannot Update User's Thumbnail Image.");
                var userIdExStr = user != null ? user.Id.ToString() : "Unauthorized User";
                logger.LogError(ex, $"Cannot Update User's Thumbnail Image. UserId: {userIdExStr}");
            }

            return uploadedFile;
        }
    }
}
