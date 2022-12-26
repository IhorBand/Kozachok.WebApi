using Kozachok.WebApi.ModelsWebApi.Authorize;

namespace Kozachok.WebApi.Services.Abstractions
{
    public interface ITokenService
    {
        public Task<AuthorizeUserOutputModel?> GenerateToken(AuthorizeUserInputModel model);
    }
}
