using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TrackingSystem.Api.AppLogic.Core;
using TrackingSystem.Api.Shared.Enums;
using TrackingSystem.Api.Shared.IManagers;

namespace TrackingSystem.Api.BusinessLogic.Managers
{
    public class JWTAuthManager : IJWTAuthManager
    {
        private const int LIFETIME_ACCESS = 600; // 10 минут
        private const int LIFETIME_REFRESH = 2_592_000; // 30 дней

        private readonly AppConfig _appConfig;

        public JWTAuthManager(IOptions<AppConfig> options)
        {
            _appConfig = options.Value;
        }

        public string GenerateToken(IEnumerable<Claim> claims, EJwtTokenType type)
        {
            var now = DateTime.UtcNow;

            var lifetime = type switch
            {
                EJwtTokenType.Access => LIFETIME_ACCESS,
                EJwtTokenType.Refresh => LIFETIME_REFRESH,
                _ => throw new NotImplementedException(type.ToString()),
            };

            var signingCredentials = new SigningCredentials(
                GetSymmetricSecurityKey(type),
                SecurityAlgorithms.HmacSha256);

            var tokenConfig = new JwtSecurityToken(
                issuer: _appConfig.JWTIssuer,
                audience: _appConfig.JWTAudience,
                notBefore: now,
                claims: claims,
                expires: now.Add(TimeSpan.FromSeconds(lifetime)),
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(tokenConfig);
        }

        public SymmetricSecurityKey GetSymmetricSecurityKey(EJwtTokenType type) =>
        type switch
            {
                EJwtTokenType.Access => new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(_appConfig.JWTAccessKey)),

                EJwtTokenType.Refresh => new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(_appConfig.JWTRefreshKey)),

                _ => throw new NotImplementedException(type.ToString())
            };
}
}
