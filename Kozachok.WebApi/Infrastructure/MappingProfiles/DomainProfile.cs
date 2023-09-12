using AutoMapper;
using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Models.DbEntities;
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

            CreateTwoWayMap<Movie, Models.Movie.Movie>();
            CreateTwoWayMap<PagedResult<Movie>, PagedResult<Models.Movie.Movie>>();

            CreateTwoWayMap<Shared.DTO.Models.DbEntities.File, Models.File.File>();
            CreateTwoWayMap<User, Models.User.User>();
            CreateTwoWayMap<Shared.DTO.Models.Result.User.UserDetails, Models.User.UserDetails>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
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
