using Kozachok.Domain.Handlers.Common;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.Abstractions.Identity;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Common;
using MediatR;
using System;
using System.Threading.Tasks;
using System.Threading;
using Kozachok.Domain.Queries.Playlist;
using Kozachok.Shared.DTO.Models.Result.Playlist;
using Kozachok.Utils.Validation;
using Kozachok.Shared;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using Kozachok.Shared.DTO.Models.DomainEntities;

namespace Kozachok.Domain.Handlers.Queries
{
    public class PlaylistQueryHandler :
        QueryHandler,
        IRequestHandler<GetPlaylistQuery, PagedResult<PlaylistMovieDto>>
    {
        private readonly IUserRepository userRepository;
        private readonly IRoomRepository roomRepository;
        private readonly IRoomUserRepository roomUserRepository;
        private readonly IPlaylistMovieRepository playlistMovieRepository;
        private readonly IPlaylistQualityRepository playlistQualityRepository;
        private readonly IUser user;
        private readonly IMapper mapper;

        public PlaylistQueryHandler(
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,
            IUserRepository userRepository,
            IRoomRepository roomRepository,
            IRoomUserRepository roomUserRepository,
            IPlaylistMovieRepository playlistMovieRepository,
            IPlaylistQualityRepository playlistQualityRepository,
            IUser user,
            IMapper mapper)
        : base(
                bus,
                notifications
        )
        {
            this.userRepository = userRepository;
            this.roomRepository = roomRepository;
            this.roomUserRepository = roomUserRepository;
            this.playlistMovieRepository = playlistMovieRepository;
            this.playlistQualityRepository = playlistQualityRepository;
            this.user = user;
            this.mapper = mapper;
        }

        public async Task<PagedResult<PlaylistMovieDto>> Handle(GetPlaylistQuery request, CancellationToken cancellationToken)
        {
            if (IsUserAuthorized(user) == false)
            {
                await Bus.InvokeDomainNotificationAsync("User is not authorized.");
                return null;
            }

            request
                .IsInvalidGuid(e => e.RoomId, async () => await Bus.InvokeDomainNotificationAsync("RoomId is invalid."));

            var currentUser = await userRepository.GetAsync(user.Id.Value);

            if (currentUser == null || currentUser.Id == Guid.Empty)
            {
                await Bus.InvokeDomainNotificationAsync("User is not authorized.");
                return null;
            }

            if (!IsValidOperation())
            {
                return null;
            }

            var room = await roomRepository.GetAsync(request.RoomId);

            if (room == null || room.Id == Guid.Empty)
            {
                await Bus.InvokeDomainNotificationAsync("Room doesn't exist.");
                return null;
            }

            if (room.RoomTypeId != Shared.DTO.Enums.RoomType.Public)
            {
                var roomUser = await roomUserRepository.FirstOrDefaultAsync(ru => ru.RoomId == request.RoomId && ru.UserId == user.Id);
                if (roomUser == null || roomUser.Id == Guid.Empty)
                {
                    await Bus.InvokeDomainNotificationAsync("User doesn't have permissions to use this room.");
                    return null;
                }
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

            var movieQuery = playlistMovieRepository.Query().Where(pm => pm.RoomId == room.Id);
            var resultMovies = playlistMovieRepository.Page(movieQuery, currentPage, itemsPerPage);

            var resultItems = new List<PlaylistMovies>();

            foreach (var movie in resultMovies.Items)
            {
                var qualities = playlistQualityRepository.Query().Where(q => q.PlaylistMovieId == movie.Id).ToList();
                
                resultItems.Add(new PlaylistMovies()
                {
                    MovieDescription = movie,
                    MovieQualities = qualities
                });
            }

            resultItems = resultItems.OrderBy(i => i.MovieDescription.OrderNumber).ToList();
            
            var result = new PagedResult<PlaylistMovies>(currentPage, resultMovies.TotalPages, resultMovies.TotalItems,
                resultMovies.PageSize, resultItems);

            return mapper.Map<PagedResult<PlaylistMovieDto>>(result);
        }
    }
}
