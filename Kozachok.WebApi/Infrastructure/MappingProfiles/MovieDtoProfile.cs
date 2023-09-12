using AutoMapper;
using Kozachok.Shared.DTO.Models.DbEntities;
using Kozachok.Shared.DTO.Models.DomainEntities;

namespace Kozachok.WebApi.Infrastructure.MappingProfiles
{
    public class MovieDtoProfile : Profile
    {
        public MovieDtoProfile()
        {
            CreateMap<Movie, MovieDto>();
            CreateMap<MovieDto, Movie>();
        }
    }
}

