using AutoMapper;
using Kozachok.Domain.Commands.Room;
using Kozachok.Domain.Queries.Room;
using Kozachok.Shared;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.DTO.Common;
using Kozachok.WebApi.Auth;
using Kozachok.WebApi.Controllers.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kozachok.WebApi.Controllers
{
    public class RoomController : UserControllerBase
    {
        public RoomController(
            IMediatorHandler mediator,
            IMapper mapper,
            INotificationHandler<DomainNotification> notifications
            ) : base(mediator, mapper, notifications)
        {
        }

        [HttpPost]
        [BearerAuthorization]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomCommand command)
        {
            var result = await bus.RequestAsync(command);
            return Response(result);
        }

        [HttpPut]
        [BearerAuthorization]
        public async Task<IActionResult> UpdateRoom([FromBody] UpdateRoomCommand command)
        {
            await bus.SendAsync(command);
            return Response();
        }

        [HttpPut("{roomId}/Join")]
        [BearerAuthorization]
        public async Task<IActionResult> JoinRoom([FromRoute] Guid roomId)
        {
            await bus.SendAsync(new JoinRoomCommand() { RoomId = roomId });
            return Response();
        }

        [HttpPut("{roomId}/Leave")]
        [BearerAuthorization]
        public async Task<IActionResult> LeaveRoom([FromRoute] Guid roomId)
        {
            await bus.SendAsync(new LeaveRoomCommand() { RoomId = roomId });
            return Response();
        }

        [HttpDelete]
        [BearerAuthorization]
        public async Task<IActionResult> DeleteRoom([FromBody] DeleteRoomCommand command)
        {
            await bus.SendAsync(command);
            return Response();
        }

        [HttpGet("{roomId}")]
        [BearerAuthorization]
        public async Task<IActionResult> GetRoom([FromRoute(Name = "roomId")] Guid roomId)
        {
            var result = await bus.RequestAsync(new GetRoomQuery() { RoomId = roomId });
            return Response(result);
        }

        [HttpGet("Public")]
        [BearerAuthorization]
        public async Task<IActionResult> GetPublicRooms([FromQuery(Name = "page")] int? page = 1, [FromQuery(Name = "itemsPerPage")] int? itemsPerPage = GlobalConstants.DefaultPageSize, [FromQuery(Name = "name")] string? name = null)
        {
            var result = await bus.RequestAsync(new GetPublicRoomsQuery() { ItemsPerPage = itemsPerPage, Page = page, Name = name });
            return Response(result);
        }

        [HttpGet("Created")]
        [BearerAuthorization]
        public async Task<IActionResult> GetCreatedRooms([FromQuery(Name = "page")] int? page = 1, [FromQuery(Name = "itemsPerPage")] int? itemsPerPage = GlobalConstants.DefaultPageSize, [FromQuery(Name = "name")] string? name = null)
        {
            var result = await bus.RequestAsync(new GetCreatedRoomsQuery() { ItemsPerPage = itemsPerPage, Page = page, Name = name });
            return Response(result);
        }

        [HttpGet("Joined")]
        [BearerAuthorization]
        public async Task<IActionResult> GetJoinedRooms([FromQuery(Name = "page")] int? page = 1, [FromQuery(Name = "itemsPerPage")] int? itemsPerPage = GlobalConstants.DefaultPageSize, [FromQuery(Name = "name")] string? name = null)
        {
            var result = await bus.RequestAsync(new GetJoinedRoomsQuery() { ItemsPerPage = itemsPerPage, Page = page, Name = name });
            return Response(result);
        }
    }
}
