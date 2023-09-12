using AutoMapper;
using Kozachok.Shared.DTO.Models.DbEntities;
using Kozachok.Shared.DTO.Models.DomainEntities;

namespace Kozachok.WebApi.Infrastructure.MappingProfiles
{
    public class UserInformationDtoProfile : Profile
    {
        public UserInformationDtoProfile()
        {
            CreateMap<User, UserInformationDto>()
                .ForMember(dest => dest.ThumbnailImageUrl,
                    opt => opt.MapFrom(
                        src => src.ThumbnailImageFile != null ? src.ThumbnailImageFile.Url : string.Empty));
        }
    }
}
