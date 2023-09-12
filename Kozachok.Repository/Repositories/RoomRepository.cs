using AutoMapper;
using Kozachok.Repository.Contexts;
using Kozachok.Repository.Repositories.Common;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Models.DbEntities;
using Kozachok.Shared.DTO.Models.DomainEntities;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Kozachok.Repository.Repositories
{
    public class RoomRepository : CrudRepository<Room>, IRoomRepository
    {
        private readonly IMapper mapper;

        public RoomRepository(MainDbContext context, IMapper mapper) : base(context)
        {
            this.mapper = mapper;
        }

        public async Task<RoomFullInformationDto> GetRoomFullInformationDtoAsync(Guid roomId, CancellationToken ct = default)
        {
            var roomDataModel = await Query()
                .Include(r => r.RoomUsers)
                .FirstOrDefaultAsync(r => r.Id == roomId, ct);

            if (roomDataModel == null)
            {
                return null;
            }

            var usersInRoom = roomDataModel.RoomUsers.Select(ru => ru.User).ToList();
            var userAdmin = roomDataModel.RoomUsers.Select(ru => ru.User).First(u => u.Id == roomDataModel.OwnerUserId);

            return new RoomFullInformationDto
            {
                Room = mapper.Map<RoomDto>(roomDataModel),
                UsersInRoom = mapper.Map<List<UserInformationDto>>(usersInRoom),
                UserAdmin = mapper.Map<UserInformationDto>(userAdmin)
            };
        }
    }
}
