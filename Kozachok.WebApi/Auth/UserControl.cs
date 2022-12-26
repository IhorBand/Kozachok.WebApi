using Kozachok.Shared.Abstractions.Identity;
using System.Security.Claims;

namespace Kozachok.WebApi.Auth
{
    public class UserControl : IUser
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserControl(IHttpContextAccessor httpContextAccessor) => this.httpContextAccessor = httpContextAccessor;

        public string? Id => httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier)?.Value;

        public string? Name => httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(e => e.Type == ClaimTypes.Name)?.Value;

        public string? Email => httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(e => e.Type == ClaimTypes.Email)?.Value;
    }
}
