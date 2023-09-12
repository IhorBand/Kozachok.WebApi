using AutoMapper;
using Kozachok.Shared.DTO.Models.DbEntities;
using Kozachok.Shared.DTO.Models.DomainEntities;

namespace Kozachok.WebApi.Infrastructure.MappingProfiles
{
    public class RoomFullInformationDtoProfile : Profile
    {
        public RoomFullInformationDtoProfile()
        {
            CreateMap<Room, RoomFullInformationDto>()
                .ForMember(dest => dest.Room,
                    opt => opt.MapFrom(
                        src => src));
        }
    }
}
