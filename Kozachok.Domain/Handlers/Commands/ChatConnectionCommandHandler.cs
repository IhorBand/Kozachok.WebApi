using Kozachok.Domain.Handlers.Common;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.Abstractions.Identity;
using Kozachok.Shared.Abstractions.Repositories.Common;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Models.DbEntities;
using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Kozachok.Domain.Commands.ChatConnection;
using Kozachok.Utils.Validation;

namespace Kozachok.Domain.Handlers.Commands
{
    public class ChatConnectionCommandHandler :
        CommandHandler,
        IRequestHandler<JoinRoomChatCommand>
    {
        private readonly IRoomRepository roomRepository;
        private readonly IChatConnectionRepository chatConnectionRepository;
        private readonly IUser user;

        public ChatConnectionCommandHandler(
            IUnitOfWork uow,
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,
            IRoomRepository roomRepository,
            IChatConnectionRepository chatConnectionRepository,
            IUser user
        )
        : base(
                uow,
                bus,
                notifications
        )
        {
            this.roomRepository = roomRepository;
            this.chatConnectionRepository = chatConnectionRepository;
            this.user = user;
        }

        public async Task<Unit> Handle(JoinRoomChatCommand request, CancellationToken cancellationToken)
        {
            request
                .IsInvalidGuid(e => e.RoomId, async () => await Bus.InvokeDomainNotificationAsync("RoomId is invalid."))
                .IsInvalidGuid(e => e.UserId, async () => await Bus.InvokeDomainNotificationAsync("UserId is invalid."));

            if (!IsValidOperation())
            {
                return Unit.Value;
            }
            
            var isUserHavePermissionToJoinChat = await roomRepository
                .AnyAsync(r => r.Id == request.RoomId && r.RoomUsers.Any(ru => ru.UserId == user.Id), 
                    cancellationToken);

            if (!isUserHavePermissionToJoinChat)
            {
                await Bus.InvokeDomainNotificationAsync("User must join room before use chat.");
                return Unit.Value;
            }

            var connection = await chatConnectionRepository.FirstOrDefaultAsync(cc => cc.UserId == request.UserId && cc.RoomId == request.RoomId, cancellationToken);

            if (connection != null && connection.Id != Guid.Empty)
            {
                await chatConnectionRepository.DeleteAsync(connection.Id, cancellationToken);
            }

            var newConnection = ChatConnection.Create(request.UserId, request.RoomId, request.ConnectionId);

            await chatConnectionRepository.AddAsync(newConnection, cancellationToken);
            Commit();

            return Unit.Value;
        }
    }
}
