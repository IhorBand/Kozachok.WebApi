using AutoMapper;
using Kozachok.Domain.Commands.Playlist;
using Kozachok.Domain.Queries.Playlist;
using Kozachok.Shared;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.DTO.Common;
using Kozachok.WebApi.Auth;
using Kozachok.WebApi.Controllers.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kozachok.WebApi.Controllers
{
    public class PlaylistContoller : UserControllerBase
    {
        public PlaylistContoller(
            IMediatorHandler mediator,
            IMapper mapper,
            INotificationHandler<DomainNotification> notifications
            ) : base(mediator, mapper, notifications)
        {
        }

        [HttpGet("Room/{roomId}")]
        [BearerAuthorization]
        public async Task<IActionResult> GetPlaylist(
            [FromRoute] Guid roomId,
            [FromQuery(Name = "page")] int? page = 1,
            [FromQuery(Name = "itemsPerPage")] int? itemsPerPage = GlobalConstants.DefaultPageSize)
        {
            var result = await Bus.RequestAsync(new GetPlaylistQuery()
            {
                Page = page,
                ItemsPerPage = itemsPerPage,
                RoomId = roomId
            });
            return Response(result);
        }

        [HttpPost]
        [BearerAuthorization]
        public async Task<IActionResult> AddMovieToPlaylist([FromBody] AddMovieToPlaylistCommand command)
        {
            await Bus.SendAsync(command);
            return Response();
        }

        [HttpDelete]
        [BearerAuthorization]
        public async Task<IActionResult> DeleteMovieFromPlaylist([FromBody] DeleteMovieFromPlaylistCommand command)
        {
            await Bus.SendAsync(command);
            return Response();
        }
    }
}
