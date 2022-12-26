using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Kozachok.WebApi.ModelsWebApi.Authorize;
using Kozachok.WebApi.Services.Abstractions;
using Kozachok.Shared.DTO.Configuration;
using Kozachok.Shared.Abstractions.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Security.Principal;

namespace Kozachok.WebApi.Services.Implementation
{
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> logger;
        private readonly IUserRepository userRepository;
        private readonly IDistributedCache cache;
        private readonly JwtTokenConfiguration jwtTokenConfiguration;

        public TokenService(
            ILogger<TokenService> logger,
            IUserRepository userRepository,
            IDistributedCache cache,
            JwtTokenConfiguration jwtTokenConfiguration)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.jwtTokenConfiguration = jwtTokenConfiguration ?? throw new ArgumentNullException(nameof(jwtTokenConfiguration));
        }

        public async Task<AuthorizeUserOutputModel?> GenerateToken(AuthorizeUserInputModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                return null;
            }

            var user = await this.userRepository.FirstOrDefaultAsync(u => u.Email == model.Email);
            
            if (user != null)
            {
                if (!user.CheckPassword(model.Password))
                {
                    if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.RefreshToken))
                    {
                        return null;
                    }

                    var storedToken = cache.GetString(model.RefreshToken);

                    if (string.IsNullOrWhiteSpace(storedToken))
                    {
                        return null;
                    }

                    var refreshTokenDataStored = JsonConvert.DeserializeObject<RefreshTokenData>(storedToken);

                    if (refreshTokenDataStored == null)
                    {
                        return null;
                    }

                    if (model.Email != refreshTokenDataStored.Email || model.RefreshToken != refreshTokenDataStored.RefreshToken)
                    { 
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }

            var now = DateTime.UtcNow;
            var expiredIn = now.AddSeconds(this.jwtTokenConfiguration.TokenExpiresIn);
            var expiresIn = expiredIn - now;
            var refreshTokenExpiredIn = now.AddSeconds(this.jwtTokenConfiguration.RefreshTokenExpiresIn);
            var refreshTokenExpiresIn = refreshTokenExpiredIn - now;

            var identity = new ClaimsIdentity
            (
                new GenericIdentity(user.Email, "Login"),
                new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, this.jwtTokenConfiguration.Subject),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, now.ToString()),
                    new Claim(JwtCustomClaimNames.UserId, user.Id.ToString()),
                    new Claim(JwtCustomClaimNames.UserName, user.Name),
                    new Claim(JwtCustomClaimNames.Email, user.Email)
                }
            );

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.jwtTokenConfiguration.Key));

            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = this.jwtTokenConfiguration.Issuer,
                Audience = this.jwtTokenConfiguration.Audience,
                SigningCredentials = signIn,
                Subject = identity,
                NotBefore = now,
                Expires = expiredIn
            });

            var tokenResult = jwtSecurityTokenHandler.WriteToken(securityToken);

            var result = new AuthorizeUserOutputModel()
            {
                ExpiresInSeconds = (int)expiresIn.TotalSeconds,
                ExpiresInUTC = expiredIn.ToString("yyyy-MM-dd HH:mm:ss"),
                IssuedInUTC = now.ToString("yyyy-MM-dd HH:mm:ss"),
                RefreshToken = Guid.NewGuid().ToString().Replace("-", String.Empty),
                TokenType = "Bearer",
                Token = tokenResult,
                UserId = user.Id.ToString(),
                UserName = user.Name,
                RefreshTokenExpiresInSeconds = (int)refreshTokenExpiresIn.TotalSeconds,
                RefreshTokenExpiresInUTC = refreshTokenExpiredIn.ToString("yyyy-MM-dd HH:mm:ss"),
                RefreshTokenIssuedInUTC = now.ToString("yyyy-MM-dd HH:mm:ss"),
                IsAuthorized = true,
                Message = "OK, pipeline works."
            };

            var cacheOptions = new DistributedCacheEntryOptions();
            cacheOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(jwtTokenConfiguration.RefreshTokenExpiresIn));

            cache.SetString
            (
                result.RefreshToken,
                JsonConvert.SerializeObject(new RefreshTokenData { RefreshToken = result.RefreshToken, Email = user.Email }),
                cacheOptions
            );

            return result;
        }
    }
}
