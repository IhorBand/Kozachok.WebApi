using Kozachok.Domain.Handlers.Common;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.Abstractions.Identity;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Common;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using Kozachok.Domain.Queries.Playlist;
using Kozachok.Shared;
using System.Linq;
using AutoMapper;
using Kozachok.Shared.DTO.Enums;
using Kozachok.Shared.DTO.Models.DomainEntities;
using Microsoft.EntityFrameworkCore;

namespace Kozachok.Domain.Handlers.Queries
{
    public class PlaylistQueryHandler :
        QueryHandler,
        IRequestHandler<GetPlaylistQuery, PagedResult<PlaylistMovieDto>>
    {
        private readonly IRoomRepository roomRepository;
        private readonly IPlaylistMovieRepository playlistMovieRepository;
        private readonly IUser user;
        private readonly IMapper mapper;

        public PlaylistQueryHandler(
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,
            IRoomRepository roomRepository,
            IPlaylistMovieRepository playlistMovieRepository,
            IUser user,
            IMapper mapper)
        : base(
                bus,
                notifications
        )
        {
            this.roomRepository = roomRepository;
            this.playlistMovieRepository = playlistMovieRepository;
            this.user = user;
            this.mapper = mapper;
        }

        public async Task<PagedResult<PlaylistMovieDto>> Handle(GetPlaylistQuery request, CancellationToken cancellationToken)
        {
            if (!IsValidOperation())
            {
                return null;
            }

            var isUserHavePermissionToViewRoom = await roomRepository
                .AnyAsync(r => r.Id == request.RoomId && (r.RoomTypeId == RoomType.Public
                               || (r.RoomTypeId == RoomType.Private 
                                   && r.RoomUsers.Any(ru => ru.UserId == user.Id))), 
                    cancellationToken);

            if (!isUserHavePermissionToViewRoom)
            {
                await Bus.InvokeDomainNotificationAsync("User doesn't have permissions to use this room.");
                return null;
            }

            var currentPage = 1;
            var itemsPerPage = GlobalConstants.DefaultPageSize;

            if (request.Page != null && request.Page > 0)
            {
                currentPage = request.Page.Value;
            }

            if (request.ItemsPerPage != null && request.ItemsPerPage > 0 && request.ItemsPerPage < itemsPerPage)
            {
                itemsPerPage = request.ItemsPerPage.Value;
            }

            var movieQuery = playlistMovieRepository
                .Query()
                .AsNoTracking()
                .Include(pm => pm.Movie)
                .Include(pm => pm.PlaylistMovieVideos)
                    .ThenInclude(pmv => pmv.PlaylistMovieVideoQualities)
                .Where(pm => pm.RoomId == request.RoomId)
                .OrderBy(pm => pm.OrderNumber);
            
            var resultMovies = await playlistMovieRepository
                .PageAsync(movieQuery, currentPage, itemsPerPage, cancellationToken);

            return mapper.Map<PagedResult<PlaylistMovieDto>>(resultMovies);
        }
    }
}
