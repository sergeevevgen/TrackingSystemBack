using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TrackingSystem.Api.DataLayer.DataAccessManagers;
using TrackingSystem.Api.Shared.Dto.User;
using TrackingSystem.Api.Shared.Enums;
using TrackingSystem.Api.Shared.IManagers;
using ILogger = NLog.ILogger;

namespace TrackingSystem.Api.AppLogic.Core
{
    public class JWTUserManager : IJWTUserManager
    {
        private readonly IJWTAuthManager _jwtAuthManager;
        private readonly ISecurityTokenValidator _securityTokenValidator;
        private readonly ILogger _logger;
        private readonly UserDbManager _dbManager;

        public JWTUserManager(
            IJWTAuthManager jwtAuthManager,
            ISecurityTokenValidator securityTokenValidator,
            ILogger logger, 
            UserDbManager dbManager)
        {
            _jwtAuthManager = jwtAuthManager;
            _securityTokenValidator = securityTokenValidator;
            _logger = logger;
            _dbManager = dbManager;
        }

        public async Task<UserResponseData> GetUserByIdentity(ClaimsIdentity identity)
        {
            if (!Guid.TryParse(identity?.FindFirst("Id")?.Value, out Guid id))
            {
                throw new Exception("Не удалось получить Id пользователя");
            };

            var result = await _dbManager.FindUser(new UserDataQuery
            {
                Id = id,
            }, default) ?? throw new Exception("Пользователь не был найден");

            return result;
        }

        /// <summary>
        /// Создание нового токена при окончании работы старого
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="SecurityTokenValidationException"></exception>
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _jwtAuthManager.GetSymmetricSecurityKey(EJwtTokenType.Refresh),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true
                };

                var principal = _securityTokenValidator.ValidateToken(
                    token,
                    tokenValidationParameters,
                    out SecurityToken securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken)
                    throw new InvalidOperationException("Ошибка получения SecurityToken");

                var jwtTokenValid = jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase);

                if (!jwtTokenValid)
                    throw new SecurityTokenValidationException("Ошибка валидации SecurityToken");

                return principal;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка получения пользователя по истекшему токену");
                throw;
            }
        }
    }
}
