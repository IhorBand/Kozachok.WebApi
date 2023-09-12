using AutoMapper;
using Kozachok.Shared.DTO.Models.DbEntities;
using Kozachok.Shared.DTO.Models.DomainEntities;

namespace Kozachok.WebApi.Infrastructure.MappingProfiles
{
    public class RoomUserDtoProfile : Profile
    {
        public RoomUserDtoProfile()
        {
            CreateMap<RoomUser, RoomUserDto>();
            CreateMap<RoomUserDto, RoomUser>();
        }
    }
}