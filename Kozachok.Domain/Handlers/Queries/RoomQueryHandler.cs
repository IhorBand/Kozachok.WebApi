using Kozachok.Domain.Handlers.Common;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.Abstractions.Identity;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Common;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using Kozachok.Domain.Queries.Room;
using Kozachok.Utils.Validation;
using System;
using Kozachok.Shared;
using System.Linq;
using AutoMapper;
using Kozachok.Shared.DTO.Enums;
using Kozachok.Shared.DTO.Models.DomainEntities;

namespace Kozachok.Domain.Handlers.Queries
{
    public class RoomQueryHandler :
        QueryHandler,
        IRequestHandler<GetRoomQuery, RoomDto>,
        IRequestHandler<GetPublicRoomsQuery, PagedResult<RoomDto>>,
        IRequestHandler<GetCreatedRoomsQuery, PagedResult<RoomDto>>,
        IRequestHandler<GetJoinedRoomsQuery, PagedResult<RoomDto>>,
        IRequestHandler<GetRoomFullInformationDtoQuery, RoomFullInformationDto>
    {
        private readonly IRoomRepository roomRepository;
        private readonly IRoomUserRepository roomUserRepository;

        private readonly IMapper mapper;
        private readonly IUser user;

        public RoomQueryHandler(
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,
            IRoomRepository roomRepository,
            IRoomUserRepository roomUserRepository,
            IMapper mapper, 
            IUser user)
            : base(
                bus,
                notifications
            )
        {
            this.roomRepository = roomRepository;
            this.roomUserRepository = roomUserRepository;
            this.mapper = mapper;
            this.user = user;
        }

        public async Task<RoomDto> Handle(GetRoomQuery request, CancellationToken cancellationToken)
        {
            request
                .IsInvalidGuid(e => e.RoomId,
                    async () => await Bus.InvokeDomainNotificationAsync("UserId is invalid."));

            if (!IsValidOperation())
            {
                return null;
            }

            var room = await roomRepository.GetAsync(request.RoomId, cancellationToken);

            if (room == null || room.Id == Guid.Empty)
            {
                await Bus.InvokeDomainNotificationAsync("Room doesn't exist.");
                return null;
            }

            if (room.RoomTypeId != RoomType.Public)
            {
                var isUserHavePermission = await roomUserRepository
                    .AnyAsync(ru => ru.RoomId == request.RoomId && (ru.Room.RoomTypeId == RoomType.Public
                                    || (ru.Room.RoomTypeId == RoomType.Private && ru.RoomId == request.RoomId &&
                                        ru.UserId == user.Id)), cancellationToken);

                if (!isUserHavePermission)
                {
                    await Bus.InvokeDomainNotificationAsync("User doesn't have permission to use this room.");
                    return null;
                }
            }

            return mapper.Map<RoomDto>(room);
        }

        public async Task<PagedResult<RoomDto>> Handle(GetPublicRoomsQuery request, CancellationToken cancellationToken)
        {
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

            var query =
                roomRepository.Query().Where(e => e.RoomTypeId == RoomType.Public);

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var lowerName = request.Name.ToLower();
                query = query.Where(e => e.Name.ToLower().Contains(lowerName));
            }

            var result = await roomRepository.PageAsync(query, currentPage, itemsPerPage, cancellationToken);

            return mapper.Map<PagedResult<RoomDto>>(result);
        }

        public async Task<PagedResult<RoomDto>> Handle(GetCreatedRoomsQuery request, CancellationToken cancellationToken)
        {
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

            var query = roomRepository.Query().Where(e => e.OwnerUserId == user.Id);

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var lowerName = request.Name.ToLower();
                query = query.Where(e => e.Name.ToLower().Contains(lowerName));
            }

            var result = await roomRepository.PageAsync(query, currentPage, itemsPerPage, cancellationToken);

            return mapper.Map<PagedResult<RoomDto>>(result);
        }

        public async Task<PagedResult<RoomDto>> Handle(GetJoinedRoomsQuery request, CancellationToken cancellationToken)
        {
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

            var query = roomUserRepository.Query()
                .Where(roomUser => roomUser.UserId == user.Id && roomUser.IsOwner == false)
                .Join(roomRepository.Query(),
                    roomUser => roomUser.RoomId,
                    room => room.Id,
                    (roomUser, room) => room);

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var lowerName = request.Name.ToLower();
                query = query.Where(e => e.Name.ToLower().Contains(lowerName));
            }

            var result = await roomRepository.PageAsync(query, currentPage, itemsPerPage, cancellationToken);
            
            return mapper.Map<PagedResult<RoomDto>>(result);
        }

        public async Task<RoomFullInformationDto> Handle(GetRoomFullInformationDtoQuery request, CancellationToken cancellationToken)
        {
            var roomFullInformationDto = await roomRepository.GetRoomFullInformationDtoAsync(request.RoomId, user.Id.Value, cancellationToken);

            return roomFullInformationDto;
        }
    }
}
