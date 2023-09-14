using AutoMapper;
using Kozachok.Shared.DTO.Models.DbEntities;
using Kozachok.Shared.DTO.Models.DomainEntities;

namespace Kozachok.WebApi.Infrastructure.MappingProfiles
{
    public class PlaylistMovieVideoDtoProfile : Profile
    {
        public PlaylistMovieVideoDtoProfile()
        {
            CreateMap<PlaylistMovieVideo, PlaylistMovieVideoDto>()
                .ForMember(dest => dest.PlaylistMovieVideoQualityDtos,
                    opt => opt.MapFrom(
                        src => src.PlaylistMovieVideoQualities));

            CreateMap<PlaylistMovieVideoDto, PlaylistMovieVideo>();
        }
    }
}
