using System.Collections.Generic;

namespace Kozachok.Shared.DTO.Models.DomainEntities
{
    public class RoomFullInformationDto
    {
        public RoomDto Room { get; set; }
        public List<UserInformationDto> UsersInRoom { get; set; }
        public List<PlaylistMovieDto> PlaylistMovies { get; set; }
        public UserInformationDto UserAdmin { get; set; }
    }
}
