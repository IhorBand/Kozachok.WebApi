using Microsoft.AspNetCore.Authorization;

namespace Kozachok.WebApi.Auth
{
    public class BearerAuthorizationAttribute : AuthorizeAttribute
    {
        public BearerAuthorizationAttribute() : base("Bearer")
        {
        }

        //TODO: Roles
        //public BearerAuthorizartionAtribute(params Role[] roles) : base("Bearer") => this.Roles = string.Join(",", roles.Select(e => e.ToString()).ToArray());
    }
}
