using AutoMapper;
using Kozachok.Shared.DTO.Models.DomainEntities;

namespace Kozachok.WebApi.Infrastructure.MappingProfiles
{
    public class FileDtoProfile : Profile
    {
        public FileDtoProfile()
        {
            CreateMap<Shared.DTO.Models.DbEntities.File, FileDto>();
            CreateMap<FileDto, Shared.DTO.Models.DbEntities.File>();
        }
    }
}
