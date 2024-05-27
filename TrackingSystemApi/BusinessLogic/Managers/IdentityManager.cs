using NLog;
using System.Security.Claims;
using TrackingSystem.Api.Shared.Dto.Identity;
using TrackingSystem.Api.Shared.Enums;
using TrackingSystem.Api.Shared.IManagers;
using TrackingSystem.Api.Shared.SharedModels;

namespace TrackingSystem.Api.BusinessLogic.Managers
{
    /// <summary>
    /// Менеджер для взаимодействия с JWT
    /// </summary>
    public class IdentityManager : IIdentityManager
    {
        private readonly ILogger _logger;
        private readonly IJWTUserManager _jwtUserManager;
        private readonly IJWTAuthManager _jwtAuthManager;

        public IdentityManager(
            ILogger logger,
            IJWTAuthManager jWTAuthManager,
            IJWTUserManager jWTUserManager)
        {
            _logger = logger; 
            _jwtUserManager = jWTUserManager;
            _jwtAuthManager = jWTAuthManager;
        }

        public ClaimsIdentity CreateIdentity(
            CreateIdentityDto query,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (query.UserId.Equals(Guid.Empty) || query.Login == null || query.Login.Equals(string.Empty))
                {
                    var errrorMessage = "Ошибка получения сущности " + $"ClaimsIdentity для пользователя {query.Login}";
                    _logger.Error(errrorMessage);
                    throw new Exception(errrorMessage);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, query.Login),
                    //new(ClaimsIdentity.DefaultRoleClaimType, user.Role.GetValueOrDefault().ToString()), TODO
                    new Claim("Id", query.UserId.ToString())
                };

                foreach (var role in query.Roles)
                {
                    claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role.ToString()));
                }

                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    "Token",
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

                return claimsIdentity;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка получения сущности " + $"ClaimsIdentity для пользователя {query.Login}");
                throw;
            }
        }

        public ResponseModel<RefreshTokenResponseDTO> RefreshToken(RefreshTokenDto command, CancellationToken cancellationToken = default)
        {
            try
            {
                // Проверка валидности refresh токена
                var principal = _jwtUserManager.GetPrincipalFromExpiredToken(command.TokenHash);

                if (principal == null)
                    return new ResponseModel<RefreshTokenResponseDTO> { ErrorMessage = "Неверный refresh токен" };

                if (principal.Identity is not ClaimsIdentity identity)
                    return new ResponseModel<RefreshTokenResponseDTO> { ErrorMessage = "Ошибка получения сущности пользователя" };

                var accessToken = _jwtAuthManager.GenerateToken(identity.Claims, JwtTokenType.Access);

                return new ResponseModel<RefreshTokenResponseDTO>
                {
                    Data = new RefreshTokenResponseDTO
                    {
                        AccessToken = accessToken,
                        Id = identity.Claims.Last().Value,
                        Login = identity.Name,
                        Role = identity.Claims.ElementAt(1).Value,
                    }
                };
            }
            catch (Exception ex)
            {
                var errorMessage = "Ошибка обновления токена";
                _logger.Error(ex, errorMessage);
                return new ResponseModel<RefreshTokenResponseDTO> { ErrorMessage = errorMessage };
            }
        }
    }
}
