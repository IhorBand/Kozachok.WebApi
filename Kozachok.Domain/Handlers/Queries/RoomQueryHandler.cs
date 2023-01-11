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
using Kozachok.Shared;
using System.Linq;

namespace Kozachok.Domain.Handlers.Queries
{
    public class RoomQueryHandler :
        QueryHandler,
        IRequestHandler<GetRoomQuery, Room>,
        IRequestHandler<GetPublicRoomsQuery, PagedResult<Room>>,
        IRequestHandler<GetCreatedRoomsQuery, PagedResult<Room>>,
        IRequestHandler<GetJoinedRoomsQuery, PagedResult<Room>>
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

        public async Task<PagedResult<Room>> Handle(GetPublicRoomsQuery request, CancellationToken cancellationToken)
        {
            if (IsUserAuthorized(user) == false)
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return null;
            }

            var currentUser = await userRepository.GetAsync(user.Id.Value);

            if (currentUser == null || (currentUser != null && currentUser.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return null;
            }

            var currentPage = 1;
            var itemsPerPage = GlobalConstants.DefaultPageSize;

            if (request.Page != null && request.Page > 0)
            {
                currentPage = request.Page.Value;
            }

            if (request.ItemsPerPage != null && request.ItemsPerPage > 0 && request.ItemsPerPage < itemsPerPage)
            {
                itemsPerPage = request.ItemsPerPage.Value;
            }

            IQueryable<Room> query = roomRepository.Query().Where(e => e.RoomTypeId == Shared.DTO.Enums.RoomType.Public);

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var lowerName = request.Name.ToLower();
                query = query.Where(e => e.Name.ToLower().Contains(lowerName));
            }

            var result = roomRepository.Page(query, currentPage, itemsPerPage);

            return result;
        }

        public async Task<PagedResult<Room>> Handle(GetCreatedRoomsQuery request, CancellationToken cancellationToken)
        {
            if (IsUserAuthorized(user) == false)
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return null;
            }

            var currentUser = await userRepository.GetAsync(user.Id.Value);

            if (currentUser == null || (currentUser != null && currentUser.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return null;
            }

            var currentPage = 1;
            var itemsPerPage = GlobalConstants.DefaultPageSize;

            if (request.Page != null && request.Page > 0)
            {
                currentPage = request.Page.Value;
            }

            if (request.ItemsPerPage != null && request.ItemsPerPage > 0 && request.ItemsPerPage < itemsPerPage)
            {
                itemsPerPage = request.ItemsPerPage.Value;
            }

            IQueryable<Room> query = roomRepository.Query().Where(e => e.OwnerUserId == currentUser.Id);

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var lowerName = request.Name.ToLower();
                query = query.Where(e => e.Name.ToLower().Contains(lowerName));
            }

            var result = roomRepository.Page(query, currentPage, itemsPerPage);

            return result;
        }

        public async Task<PagedResult<Room>> Handle(GetJoinedRoomsQuery request, CancellationToken cancellationToken)
        {
            if (IsUserAuthorized(user) == false)
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return null;
            }

            var currentUser = await userRepository.GetAsync(user.Id.Value);

            if (currentUser == null || (currentUser != null && currentUser.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return null;
            }

            var currentPage = 1;
            var itemsPerPage = GlobalConstants.DefaultPageSize;

            if (request.Page != null && request.Page > 0)
            {
                currentPage = request.Page.Value;
            }

            if (request.ItemsPerPage != null && request.ItemsPerPage > 0 && request.ItemsPerPage < itemsPerPage)
            {
                itemsPerPage = request.ItemsPerPage.Value;
            }

            IQueryable<Room> query = roomUserRepository.Query()
                .Where(roomUser => roomUser.UserId == currentUser.Id)
                .Join(roomRepository.Query(),
                    roomUser => roomUser.RoomId,
                    room => room.Id,
                    (roomUser, room) => room);

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var lowerName = request.Name.ToLower();
                query = query.Where(e => e.Name.ToLower().Contains(lowerName));
            }

            var result = roomRepository.Page(query, currentPage, itemsPerPage);

            return result;
        }
    }
}
