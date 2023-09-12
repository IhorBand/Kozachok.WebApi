using AutoMapper;
using Kozachok.Shared.DTO.Models.DbEntities;
using Kozachok.Shared.DTO.Models.DomainEntities;

namespace Kozachok.WebApi.Infrastructure.MappingProfiles
{
    public class FileServerDtoProfile : Profile
    {
        public FileServerDtoProfile()
        {
            CreateMap<FileServer, FileServerDto>();
            CreateMap<FileServerDto, FileServer>();
        }
    }
}

