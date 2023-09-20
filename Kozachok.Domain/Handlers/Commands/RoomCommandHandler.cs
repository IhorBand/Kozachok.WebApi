using Kozachok.Domain.Handlers.Common;
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
using System.Linq;
using AutoMapper;
using Kozachok.Shared.DTO.Enums;
using Kozachok.Shared.DTO.Models.DomainEntities;
using Microsoft.EntityFrameworkCore;

namespace Kozachok.Domain.Handlers.Commands
{
    public class RoomCommandHandler :
        CommandHandler,
        IRequestHandler<CreateRoomCommand, RoomDto>,
        IRequestHandler<UpdateRoomCommand>,
        IRequestHandler<DeleteRoomCommand>,
        IRequestHandler<JoinRoomCommand>,
        IRequestHandler<LeaveRoomCommand>
    {
        private readonly IRoomRepository roomRepository;
        private readonly IRoomUserRepository roomUserRepository;
        private readonly IUser user;
        private readonly IMapper mapper;

        public RoomCommandHandler(
            IUnitOfWork uow,
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,
            IRoomRepository roomRepository,
            IRoomUserRepository roomUserRepository,
            IUser user,
            IMapper mapper)
        : base(
                uow,
                bus,
                notifications
        )
        {
            this.roomRepository = roomRepository;
            this.roomUserRepository = roomUserRepository;
            this.user = user;
            this.mapper = mapper;
        }

        public async Task<RoomDto> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
        {
            request
                .IsNullEmptyOrWhitespace(e => e.Name, async () => await Bus.InvokeDomainNotificationAsync("Invalid name."))
                .IsMatchMaxLength(e => e.Name, async () => await Bus.InvokeDomainNotificationAsync("Name is too big."));

            if (!IsValidOperation() || user?.Id == null)
            {
                return null;
            }

            var room = Room.Create(request.Name, request.RoomType, user.Id.Value);
            await roomRepository.AddAsync(room, cancellationToken);

            var roomUser = RoomUser.Create(user.Id.Value, room.Id, true);
            await roomUserRepository.AddAsync(roomUser, cancellationToken);

            Commit();

            return mapper.Map<RoomDto>(room);
        }

        public async Task<Unit> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
        {
            request
                .IsInvalidGuid(e => e.RoomId, async () => await Bus.InvokeDomainNotificationAsync("Invalid Room Id"))
                .IsNullEmptyOrWhitespace(e => e.Name, async () => await Bus.InvokeDomainNotificationAsync("Invalid name."))
                .IsMatchMaxLength(e => e.Name, async () => await Bus.InvokeDomainNotificationAsync("Name is too big."));

            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            var room = await roomRepository.GetAsync(request.RoomId, cancellationToken);

            if (room == null || room.Id == Guid.Empty)
            {
                await Bus.InvokeDomainNotificationAsync("Room doesn't exist.");
                return Unit.Value;
            }

            if (room.OwnerUserId != user.Id)
            {
                await Bus.InvokeDomainNotificationAsync("User don't have permission to delete this room.");
                return Unit.Value;
            }

            //TODO: Automapper
            room.Name = request.Name;
            room.RoomTypeId = request.RoomType;

            roomRepository.Update(room);

            Commit();

            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
        {
            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            var room = await roomRepository
                .Query()
                .Include(r => r.RoomUsers)
                .SingleOrDefaultAsync(r => r.Id == request.RoomId && r.OwnerUserId == user.Id, 
                    cancellationToken);

            if (room == null || room.Id == Guid.Empty)
            {
                await Bus.InvokeDomainNotificationAsync("Room doesn't exist.");
                return Unit.Value;
            }

            roomUserRepository.DeleteRange(room.RoomUsers);
            roomRepository.Delete(room);

            Commit();

            return Unit.Value;
        }

        public async Task<Unit> Handle(JoinRoomCommand request, CancellationToken cancellationToken)
        {
            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            if (user?.Id == null)
            {
                await Bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            var isUserCanJoinRoom = await roomRepository
                .AnyAsync(r => r.Id == request.RoomId 
                               && r.RoomUsers.All(ru => ru.UserId != user.Id
                               && ((r.RoomTypeId == RoomType.Private && r.SecretToken == request.SecretToken) 
                                   || r.RoomTypeId == RoomType.Public)),
                    cancellationToken);

            if (!isUserCanJoinRoom)
            {
                await Bus.InvokeDomainNotificationAsync("User cannot join this room.");
                return Unit.Value;
            }

            var roomUser = RoomUser.Create(user.Id.Value, request.RoomId, false);
            await roomUserRepository.AddAsync(roomUser, cancellationToken);

            Commit();

            return Unit.Value;
        }

        public async Task<Unit> Handle(LeaveRoomCommand request, CancellationToken cancellationToken)
        {
            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            var roomUser = await roomUserRepository
                .Query()
                .SingleOrDefaultAsync(ru => ru.IsOwner == false 
                                            && ru.RoomId == request.RoomId 
                                            && ru.UserId == user.Id, 
                    cancellationToken);

            if (roomUser == null || roomUser.Id == Guid.Empty)
            {
                await Bus.InvokeDomainNotificationAsync("User cannot leave this room.");
                return Unit.Value;
            }

            roomUserRepository.Delete(roomUser);

            Commit();

            return Unit.Value;
        }
    }
}
