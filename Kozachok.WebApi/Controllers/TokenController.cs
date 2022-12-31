using Microsoft.AspNetCore.Mvc;
using Kozachok.WebApi.ModelsWebApi.Authorize;
using Kozachok.WebApi.Services.Abstractions;
using System.Net;
using Kozachok.WebApi.Controllers.Common;
using Kozachok.Shared.Abstractions.Bus;
using MediatR;
using Kozachok.Shared.DTO.Common;
using Kozachok.WebApi.Auth;
using Kozachok.Shared.DTO.Configuration;
using AutoMapper;

namespace Kozachok.WebApi.Controllers
{
    [Route("api/token")]
    [ApiController]
    public class TokenController : BaseController
    {
        private readonly ITokenService tokenService;

        public TokenController(
            IMediatorHandler bus,
            IMapper mapper,
            INotificationHandler<DomainNotification> notifications,
            ITokenService tokenService) : base(bus, mapper, notifications)
        {
            this.tokenService = tokenService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GetTokenAsync([FromBody] AuthorizeUserInputModel model)
        {
            var result = await this.tokenService.GenerateToken(model).ConfigureAwait(false);

            if (result != null && (result.IsAuthorized == false || result.IsActive == false))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, result);
            }
            else if (result == null)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new AuthorizeUserOutputModel { IsAuthorized = false, IsActive = false, Message = "Authentication failed!" });
            }

            return Response(result);
        }
    }
}
