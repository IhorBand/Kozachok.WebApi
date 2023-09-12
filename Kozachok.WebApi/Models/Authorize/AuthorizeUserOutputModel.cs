namespace Kozachok.WebApi.ModelsWebApi.Authorize
{
    public class AuthorizeUserOutputModel
    {
        public bool IsAuthorized { get; set; }
        public bool IsActive { get; set; }
        public string? UserId { get; set; }
        public string? UserEmail { get; set; }
        public string? UserName { get; set; }
        public string? Token { get; set; }
        public string? TokenType { get; set; }
        public int ExpiresInSeconds { get; set; }
        public string? ExpiresInUtc { get; set; }
        public string? IssuedInUtc { get; set; }
        public string? RefreshToken { get; set; }
        public int RefreshTokenExpiresInSeconds { get; set; }
        public string? RefreshTokenExpiresInUtc { get; set; }
        public string? RefreshTokenIssuedInUtc { get; set; }
        public string? Message { get; set; }
    }
}
