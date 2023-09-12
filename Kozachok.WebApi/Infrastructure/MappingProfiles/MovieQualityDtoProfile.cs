using AutoMapper;
using Kozachok.Shared.DTO.Models.DbEntities;
using Kozachok.Shared.DTO.Models.DomainEntities;

namespace Kozachok.WebApi.Infrastructure.MappingProfiles
{
    public class MovieQualityDtoProfile : Profile
    {
        public MovieQualityDtoProfile()
        {
            CreateMap<PlaylistQuality, MovieQualityDto>();
            CreateMap<MovieQualityDto, PlaylistQuality>();
        }
    }
}