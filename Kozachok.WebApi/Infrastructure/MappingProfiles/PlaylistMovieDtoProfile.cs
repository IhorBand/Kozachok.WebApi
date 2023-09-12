using AutoMapper;
using Kozachok.Shared.DTO.Models.DbEntities;
using Kozachok.Shared.DTO.Models.DomainEntities;

namespace Kozachok.WebApi.Infrastructure.MappingProfiles
{
    public class PlaylistMovieDtoProfile : Profile
    {
        public PlaylistMovieDtoProfile()
        {
            CreateMap<PlaylistMovie, PlaylistMovieDto>();
            CreateMap<PlaylistMovieDto, PlaylistMovie>();
        }
    }
}