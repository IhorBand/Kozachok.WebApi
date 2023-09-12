using AutoMapper;
using Kozachok.Shared.DTO.Models.DbEntities;
using Kozachok.Shared.DTO.Models.DomainEntities;

namespace Kozachok.WebApi.Infrastructure.MappingProfiles
{
    public class TranslatorDtoProfile : Profile
    {
        public TranslatorDtoProfile()
        {
            CreateMap<Translator, TranslatorDto>();
            CreateMap<TranslatorDto, Translator>();
        }
    }
}