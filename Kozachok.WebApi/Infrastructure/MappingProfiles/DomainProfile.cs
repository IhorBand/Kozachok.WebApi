namespace Kozachok.WebApi.Infrastructure.MappingProfiles
{
    using AutoMapper;

    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
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
