using Kozachok.Domain.Handlers.Common;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.Abstractions.Identity;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Common;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using Kozachok.Shared.DTO.Models.DbEntities;
using Kozachok.Domain.Queries.Room;
using Kozachok.Utils.Validation;
using System;

namespace Kozachok.Domain.Handlers.Queries
{
    public class RoomQueryHandler :
        QueryHandler,
        IRequestHandler<GetRoomQuery, Room>
    {
        private readonly IUserRepository userRepository;
        private readonly IRoomRepository roomRepository;
        private readonly IRoomUserRepository roomUserRepository;

        private readonly IUser user;

        public RoomQueryHandler(
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,
            IUserRepository userRepository,
            IRoomRepository roomRepository,
            IRoomUserRepository roomUserRepository,
            IUser user
        )
        : base(
                bus,
                notifications
        )
        {
            this.userRepository = userRepository;
            this.roomRepository = roomRepository;
            this.roomUserRepository = roomUserRepository;
            this.user = user;
        }

        public async Task<Room> Handle(GetRoomQuery request, CancellationToken cancellationToken)
        {
            if (IsUserAuthorized(user) == false)
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return null;
            }

            request
                .IsInvalidGuid(e => e.RoomId, async () => await bus.InvokeDomainNotificationAsync("UserId is invalid."));

            if (!IsValidOperation())
            {
                return null;
            }
            
            var currentUser = await userRepository.GetAsync(user.Id.Value);

            if (currentUser == null || (currentUser != null && currentUser.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return null;
            }

            var room = await roomRepository.GetAsync(request.RoomId);

            if (room == null || (room != null && room.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("Room doesn't exist.");
                return null;
            }

            if (room.RoomTypeId == Shared.DTO.Enums.RoomType.Public)
            {
                return room;
            }
            else
            {
                var userInRoom = await roomUserRepository.FirstOrDefaultAsync(e => e.UserId == user.Id.Value && e.RoomId == room.Id);
                if (userInRoom == null || (userInRoom != null && userInRoom.Id == Guid.Empty))
                {
                    await bus.InvokeDomainNotificationAsync("User doesn't have persmission to view this room.");
                    return null;
                }
                return room;
            }
        }
    }
}
