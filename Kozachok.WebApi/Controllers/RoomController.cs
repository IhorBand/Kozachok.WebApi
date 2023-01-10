using AutoMapper;
using Kozachok.Domain.Commands.File;
using Kozachok.Domain.Commands.Room;
using Kozachok.Domain.Queries.Room;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.DTO.Common;
using Kozachok.WebApi.Auth;
using Kozachok.WebApi.Controllers.Common;
using Kozachok.WebApi.Validation;
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
    }
}
