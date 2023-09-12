using AutoMapper;
using Kozachok.Shared.DTO.Models.DbEntities;
using Kozachok.Shared.DTO.Models.DomainEntities;

namespace Kozachok.WebApi.Infrastructure.MappingProfiles
{
    public class ChatConnectionDtoProfile : Profile
    { 
        public ChatConnectionDtoProfile()
        {
            CreateMap<ChatConnection, ChatConnectionDto>();
            CreateMap<ChatConnectionDto, ChatConnection>();
        }
    }
}
