using AutoMapper;
using Kozachok.Shared.DTO.Models.DbEntities;
using Kozachok.Shared.DTO.Models.DomainEntities;

namespace Kozachok.WebApi.Infrastructure.MappingProfiles
{
    public class PlaylistMovieDtoProfile : Profile
    {
        public PlaylistMovieDtoProfile()
        {
            CreateMap<PlaylistMovie, PlaylistMovieDto>()
                .ForMember(dest => dest.Movie,
                    opt => opt.MapFrom(
                        src => src.Movie))
                .ForMember(dest => dest.PlaylistMovieVideoDtOs,
                    opt => opt.MapFrom(
                        src => src.PlaylistMovieVideos));

            ;
            CreateMap<PlaylistMovieDto, PlaylistMovie>();
        }
    }
}