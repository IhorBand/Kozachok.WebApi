﻿using Kozachok.Domain.Handlers.Common;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.Abstractions.Identity;
using Kozachok.Shared.Abstractions.Repositories.Common;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Common;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using Kozachok.Domain.Commands.Room;
using Kozachok.Shared.DTO.Models.DbEntities;
using Kozachok.Utils.Validation;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Kozachok.Domain.Handlers.Commands
{
    public class RoomCommandHandler :
        CommandHandler,
        IRequestHandler<CreateRoomCommand, Room>,
        IRequestHandler<UpdateRoomCommand>,
        IRequestHandler<DeleteRoomCommand>,
        IRequestHandler<JoinRoomCommand>,
        IRequestHandler<LeaveRoomCommand>
    {
        private readonly IUserRepository userRepository;
        private readonly IRoomRepository roomRepository;
        private readonly IRoomUserRepository roomUserRepository;
        private readonly IUser user;

        public RoomCommandHandler(
            IUnitOfWork uow,
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,
            IUserRepository userRepository,
            IRoomRepository roomRepository,
            IRoomUserRepository roomUserRepository,
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
            this.user = user;
        }

        public async Task<Room> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
        {
            if(!IsUserAuthorized(user))
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return null;
            }

            request
                .IsNullEmptyOrWhitespace(e => e.Name, async () => await bus.InvokeDomainNotificationAsync("Invalid name."))
                .IsMatchMaxLength(e => e.Name, async () => await bus.InvokeDomainNotificationAsync("Name is too big."));

            var currentUser = await userRepository.GetAsync(user.Id.Value);

            if(currentUser == null || (currentUser != null && currentUser.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return null;
            }

            if (!IsValidOperation())
            {
                return null;
            }

            var room = new Room(request.Name, request.RoomType, currentUser.Id);
            await roomRepository.AddAsync(room);

            var roomUser = new RoomUser(user.Id.Value, room.Id, true);
            await roomUserRepository.AddAsync(roomUser);

            Commit();

            return room;
        }

        public async Task<Unit> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
        {
            if (!IsUserAuthorized(user))
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            request
                .IsInvalidGuid(e => e.RoomId, async () => await bus.InvokeDomainNotificationAsync("Invalid Room Id"))
                .IsNullEmptyOrWhitespace(e => e.Name, async () => await bus.InvokeDomainNotificationAsync("Invalid name."))
                .IsMatchMaxLength(e => e.Name, async () => await bus.InvokeDomainNotificationAsync("Name is too big."));

            var currentUser = await userRepository.GetAsync(user.Id.Value);

            if (currentUser == null || (currentUser != null && currentUser.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            var room = await roomRepository.GetAsync(request.RoomId);

            if (room == null || (room != null && room.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("Room doesn't exist.");
                return Unit.Value;
            }

            if (room.OwnerUserId != currentUser.Id)
            {
                await bus.InvokeDomainNotificationAsync("User don't have permission to delete this room.");
                return Unit.Value;
            }

            room.Name = request.Name;
            room.RoomTypeId = request.RoomType;

            await roomRepository.UpdateAsync(room);

            Commit();

            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
        {
            if (!IsUserAuthorized(user))
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            request
                .IsInvalidGuid(e => e.RoomId, async () => await bus.InvokeDomainNotificationAsync("Invalid Room Id."));

            var currentUser = await userRepository.GetAsync(user.Id.Value);

            if (currentUser == null || (currentUser != null && currentUser.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            var room = await roomRepository.GetAsync(request.RoomId);

            if (room == null || (room != null && room.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("Room doesn't exist.");
                return Unit.Value;
            }

            var userRoomArr = await roomUserRepository.Query(e => e.RoomId == room.Id).ToListAsync();

            if (room.OwnerUserId != currentUser.Id)
            {
                await bus.InvokeDomainNotificationAsync("User don't have permission to delete this room.");
                return Unit.Value;
            }

            if (userRoomArr != null && userRoomArr.Count > 0)
            {
                foreach(var userRoom in userRoomArr)
                {
                    await roomUserRepository.DeleteAsync(userRoom.Id);
                }

                Commit();
            }

            await roomRepository.DeleteAsync(room.Id);

            Commit();

            return Unit.Value;
        }

        public async Task<Unit> Handle(JoinRoomCommand request, CancellationToken cancellationToken)
        {
            if (!IsUserAuthorized(user))
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            request
                .IsInvalidGuid(e => e.RoomId, async () => await bus.InvokeDomainNotificationAsync("Invalid Room Id"));

            var currentUser = await userRepository.GetAsync(user.Id.Value);

            if (currentUser == null || (currentUser != null && currentUser.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            var room = await roomRepository.GetAsync(request.RoomId);

            if (room == null || (room != null && room.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("Room doesn't exist.");
                return Unit.Value;
            }

            var isUserAlreadyJoinedRoom = await roomUserRepository.AnyAsync(e => e.RoomId == room.Id && e.UserId == currentUser.Id);

            if (isUserAlreadyJoinedRoom)
            {
                await bus.InvokeDomainNotificationAsync("User already in this room.");
                return Unit.Value;
            }

            var roomUser = new RoomUser(user.Id.Value, room.Id, false);
            await roomUserRepository.AddAsync(roomUser);

            Commit();

            return Unit.Value;
        }

        public async Task<Unit> Handle(LeaveRoomCommand request, CancellationToken cancellationToken)
        {
            if (!IsUserAuthorized(user))
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            request
                .IsInvalidGuid(e => e.RoomId, async () => await bus.InvokeDomainNotificationAsync("Invalid Room Id"));

            var currentUser = await userRepository.GetAsync(user.Id.Value);

            if (currentUser == null || (currentUser != null && currentUser.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            var room = await roomRepository.GetAsync(request.RoomId);

            if (room == null || (room != null && room.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("Room doesn't exist.");
                return Unit.Value;
            }

            if(room.OwnerUserId == currentUser.Id)
            {
                await bus.InvokeDomainNotificationAsync("User is owner and cannot leave this room.");
                return Unit.Value;
            }

            var roomUser = await roomUserRepository.FirstOrDefaultAsync(e => e.RoomId == room.Id && e.UserId == currentUser.Id);

            if (roomUser == null || (roomUser != null && roomUser.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("User must join this room first.");
                return Unit.Value;
            }

            await roomUserRepository.DeleteAsync(roomUser.Id);

            Commit();

            return Unit.Value;
        }
    }
}
