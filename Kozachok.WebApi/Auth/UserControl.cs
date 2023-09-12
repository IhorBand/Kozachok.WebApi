using Kozachok.Shared.Abstractions.Identity;
using Kozachok.Shared.DTO.Configuration;

namespace Kozachok.WebApi.Auth
{
    public class UserControl : IUser
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserControl(IHttpContextAccessor httpContextAccessor) => this.httpContextAccessor = httpContextAccessor;

        public Guid? Id
        {
            get
            {
                Guid guid;
                Guid.TryParse(GetClaimValue(JwtCustomClaimNames.UserId), out guid);
                return guid;
            }
        }

        public string? Name
        {
            get
            {
                return GetClaimValue(JwtCustomClaimNames.UserName);
            }
        }

        public string? Email
        {
            get
            {
                return GetClaimValue(JwtCustomClaimNames.Email);
            }
        }

        private string? GetClaimValue(string claimName)
        {
            var claim = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(e => e.Type == claimName);
            return claim?.Value;
        }
    }
}
