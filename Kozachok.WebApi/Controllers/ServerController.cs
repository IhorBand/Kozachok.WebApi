using AutoMapper;
using Kozachok.Domain.Handlers.Queries;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Models.DbEntities;
using Kozachok.WebApi.Controllers.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kozachok.WebApi.Controllers
{
    public class ServerController : UserControllerBase
    {
        public ServerController(
            IMediatorHandler mediator,
            IMapper mapper,
            INotificationHandler<DomainNotification> notifications
            ) : base(mediator, mapper, notifications)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Post()
        {
            var result = await Bus.RequestAsync<ScriptProgress>(new GetScriptProgressQuery());
            return Response(result);
        }
    }
}
