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
using Kozachok.Shared.DTO.Enums;
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

        public async Task<RoomFullInformationDto> GetRoomFullInformationDtoAsync(Guid roomId, Guid currentUserId, CancellationToken cancellationToken = default)
        {
            var roomDataModel = await Query()
                .Include(r => r.RoomUsers)
                .Include(r => r.PlaylistMovies)
                .Where(r => r.Id == roomId && (r.RoomTypeId == RoomType.Public 
                            || (r.RoomTypeId == RoomType.Private && r.RoomUsers.Any(ru => ru.UserId == currentUserId))))
                .FirstOrDefaultAsync(r => r.Id == roomId, cancellationToken);

            if (roomDataModel == null)
            {
                return null;
            }

            var usersInRoom = roomDataModel.RoomUsers.Select(ru => ru.User).ToList();
            var userAdmin = roomDataModel.RoomUsers.Where(ru => ru.IsOwner).Select(ru => ru.User).First();

            return new RoomFullInformationDto
            {
                Room = mapper.Map<RoomDto>(roomDataModel),
                UsersInRoom = mapper.Map<List<UserInformationDto>>(usersInRoom),
                UserAdmin = mapper.Map<UserInformationDto>(userAdmin),
                PlaylistMovies = mapper.Map<List<PlaylistMovieDto>>(roomDataModel.PlaylistMovies)
            };
        }
    }
}
