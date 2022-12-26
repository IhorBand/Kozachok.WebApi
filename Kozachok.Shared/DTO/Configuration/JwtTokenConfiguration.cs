namespace Kozachok.Shared.DTO.Configuration
{
    public class JwtTokenConfiguration
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Subject { get; set; }
        public int TokenExpiresIn { get; set; }
        public int RefreshTokenExpiresIn { get; set; }
    }
}
