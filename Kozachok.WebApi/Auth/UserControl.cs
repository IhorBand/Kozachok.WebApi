using Kozachok.Shared.Abstractions.Identity;
using Kozachok.Shared.DTO.Configuration;
using System.Security.Claims;

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
                var guid = Guid.Empty;
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
            if(httpContextAccessor.HttpContext != null && httpContextAccessor.HttpContext.User != null && httpContextAccessor.HttpContext.User.Claims != null)
            {
                var claim = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(e => e.Type == claimName);
                if(claim != null)
                {
                    return claim.Value;
                }
            }

            return null;
        }
    }
}
