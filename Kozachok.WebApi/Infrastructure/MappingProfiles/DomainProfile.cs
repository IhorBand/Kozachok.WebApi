using AutoMapper;
using Kozachok.Shared.DTO.Common;
using Kozachok.WebApi.Models.Common;

namespace Kozachok.WebApi.Infrastructure.MappingProfiles
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            CreateMap<DomainNotification, ErrorMessage>()
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.DateTime));
        }

        public void CreateTwoWayMap<T1, T2>()
        {
            CreateMap<T1, T2>();
            CreateMap<T2, T1>();
        }

        public void CreateTwoWayMap(Type t1, Type t2)
        {
            CreateMap(t1, t2);
            CreateMap(t2, t1);
        }
    }
}
