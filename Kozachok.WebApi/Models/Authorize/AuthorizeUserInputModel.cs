namespace Kozachok.WebApi.ModelsWebApi.Authorize
{
    public class AuthorizeUserInputModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? RefreshToken { get; set; }
        public bool RememberMe { get; set; } = false;
    }
}
