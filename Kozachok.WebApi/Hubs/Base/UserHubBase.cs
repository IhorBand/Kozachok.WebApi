using Microsoft.AspNetCore.SignalR;
using Kozachok.Shared.DTO.Configuration;

namespace Kozachok.WebApi.Hubs.Base
{
    public class UserHubBase : Hub
    {
        public Guid UserId
        {
            get
            {
                var idStr = this.GetClaimValue(JwtCustomClaimNames.UserId);
                return new Guid(idStr);
            }
        }

        public string Email { get => this.GetClaimValue(JwtCustomClaimNames.Email); }

        public string UserName { get => this.GetClaimValue(JwtCustomClaimNames.UserName); }

        private string GetClaimValue(string claimName)
        {
            if (this.Context.User != null)
            {
                var claim = this.Context.User.Claims.FirstOrDefault(c => c.Type == claimName);
                if (claim != null)
                {
                    return claim.Value;
                }
            }

            return string.Empty;
        }
    }
}
