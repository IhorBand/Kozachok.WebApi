using AutoMapper;
using Kozachok.Shared.DTO.Models.DbEntities;
using Kozachok.Shared.DTO.Models.DomainEntities;

namespace Kozachok.WebApi.Infrastructure.MappingProfiles
{
    public class PlaylistMovieVideoQualityDtoProfile : Profile
    {
        public PlaylistMovieVideoQualityDtoProfile()
        {
            CreateMap<PlaylistMovieVideoQuality, PlaylistMovieVideoQualityDto>();
            CreateMap<PlaylistMovieVideoQualityDto, PlaylistMovieVideoQuality>();
        }
    }
}