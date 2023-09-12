using Kozachok.Domain.Handlers.Common;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.Abstractions.Identity;
using Kozachok.Shared.Abstractions.Repositories.Common;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Models.DbEntities;
using MediatR;
using System;
using System.Threading.Tasks;
using System.Threading;
using Kozachok.Domain.Commands.ChatConnection;
using Kozachok.Utils.Validation;

namespace Kozachok.Domain.Handlers.Commands
{
    public class ChatConnectionCommandHandler :
        CommandHandler,
        IRequestHandler<JoinRoomCommand>
    {
        private readonly IUserRepository userRepository;
        private readonly IRoomRepository roomRepository;
        private readonly IRoomUserRepository roomUserRepository;
        private readonly IChatConnectionRepository chatConnectionRepository;
        private readonly IUser user;

        public ChatConnectionCommandHandler(
            IUnitOfWork uow,
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,
            IUserRepository userRepository,
            IRoomRepository roomRepository,
            IRoomUserRepository roomUserRepository,
            IChatConnectionRepository chatConnectionRepository,
            IUser user
        )
        : base(
                uow,
                bus,
                notifications
        )
        {
            this.userRepository = userRepository;
            this.roomRepository = roomRepository;
            this.roomUserRepository = roomUserRepository;
            this.chatConnectionRepository = chatConnectionRepository;
            this.user = user;
        }

        public async Task<Unit> Handle(JoinRoomCommand request, CancellationToken cancellationToken)
        {
            if (IsUserAuthorized(user) == false)
            {
                await Bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            request
                .IsInvalidGuid(e => e.RoomId, async () => await Bus.InvokeDomainNotificationAsync("RoomId is invalid."))
                .IsInvalidGuid(e => e.UserId, async () => await Bus.InvokeDomainNotificationAsync("UserId is invalid."));

            var currentUser = await userRepository.GetAsync(user.Id.Value);

            if (currentUser == null || currentUser.Id == Guid.Empty)
            {
                await Bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            var room = await roomRepository.GetAsync(request.RoomId);

            if (room == null || room.Id == Guid.Empty)
            {
                await Bus.InvokeDomainNotificationAsync("Room doesn't exist.");
                return Unit.Value;
            }

            var roomUser = await roomUserRepository.FirstOrDefaultAsync(ru => ru.UserId == user.Id && ru.RoomId == request.RoomId);

            if (roomUser == null || roomUser.Id == Guid.Empty)
            {
                await Bus.InvokeDomainNotificationAsync("User doesn't have permission to use this room.");
                return Unit.Value;
            }

            var connection = await chatConnectionRepository.FirstOrDefaultAsync(cc => cc.UserId == request.UserId && cc.RoomId == request.RoomId);

            if (connection != null && connection.Id != Guid.Empty)
            {
                await chatConnectionRepository.DeleteAsync(connection.Id);
                Commit();
            }

            var newConnection = ChatConnection.Create(request.UserId, request.RoomId, request.ConnectionId);

            await chatConnectionRepository.AddAsync(newConnection);
            Commit();

            return Unit.Value;
        }
    }
}
