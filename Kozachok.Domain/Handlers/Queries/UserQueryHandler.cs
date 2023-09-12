using Kozachok.Domain.Handlers.Common;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.Abstractions.Identity;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Common;
using MediatR;
using System;
using System.Threading.Tasks;
using System.Threading;
using Kozachok.Domain.Queries.User;
using Kozachok.Shared.DTO.Models.Result.User;
using Kozachok.Utils.Validation;
using Kozachok.Shared.DTO.Models.DbEntities;

namespace Kozachok.Domain.Handlers.Queries
{
    public class UserQueryHandler :
        QueryHandler,
        IRequestHandler<GetUserDetailQuery, UserDetails>,
        IRequestHandler<GetScriptProgressQuery, ScriptProgress>
    {
        private readonly IUserRepository userRepository;
        private readonly IFileRepository fileRepository;
        private readonly IScriptProgressRepository scriptProgressRepository;

        private readonly IUser user;

        public UserQueryHandler(
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,
            IUserRepository userRepository,
            IFileRepository fileRepository,
            IScriptProgressRepository scriptProgressRepository,
            IUser user
        )
        : base(
                bus,
                notifications
        )
        {
            this.userRepository = userRepository;
            this.fileRepository = fileRepository;
            this.scriptProgressRepository = scriptProgressRepository;
            this.user = user;
        }

        public async Task<ScriptProgress> Handle(GetScriptProgressQuery request, CancellationToken cancellationToken)
        {
            var scriptProgress = await scriptProgressRepository.FirstOrDefaultAsync(q => true);

            return scriptProgress;
        }

        public async Task<UserDetails> Handle(GetUserDetailQuery request, CancellationToken cancellationToken)
        {
            if(IsUserAuthorized(user) == false)
            {
                await Bus.InvokeDomainNotificationAsync("User is not authorized.");
                return null;
            }

            request
                .IsInvalidGuid(e => e.UserId, async () => await Bus.InvokeDomainNotificationAsync("UserId is invalid."));

            if (!IsValidOperation())
            {
                return null;
            }

            var requestedUser = await userRepository.GetAsync(request.UserId);

            if (requestedUser == null || (requestedUser.Id == Guid.Empty))
            {
                await Bus.InvokeDomainNotificationAsync("User not found.");
                return null;
            }

            var model = new UserDetails()
            {
                User = requestedUser
            };

            if (requestedUser.ThumbnailImageFileId != null && requestedUser.ThumbnailImageFileId.Value != Guid.Empty)
            {
                var file = await fileRepository.GetAsync(requestedUser.ThumbnailImageFileId.Value);
                if(file != null && file.Id != Guid.Empty)
                {
                    model.ThumbnailImageUrl = file.Url;
                }
            }

            return model;
        }
    }
}
