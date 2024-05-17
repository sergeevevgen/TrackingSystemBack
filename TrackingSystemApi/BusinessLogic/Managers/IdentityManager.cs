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

        public async Task<ClaimsIdentity> CreateIdentity(
            CreateIdentityCommand query,
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
                    new(ClaimsIdentity.DefaultNameClaimType, query.Login), 
                    //new(ClaimsIdentity.DefaultRoleClaimType, user.Role.GetValueOrDefault().ToString()), TODO
                    new("Id", query.UserId.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    "Token",
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

                return await Task.FromResult(claimsIdentity);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка получения сущности " + $"ClaimsIdentity для пользователя {query.Login}");
                throw;
            }
        }

        public Task<ResponseModel<RefreshTokenResponseDTO>> RefreshToken(RefreshTokenCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                // Проверка валидности refresh токена
                var principal = _jwtUserManager.GetPrincipalFromExpiredToken(command.TokenHash);

                if (principal == null)
                    return Task.FromResult(new ResponseModel<RefreshTokenResponseDTO> { ErrorMessage = "Неверный refresh токен." });

                if (principal.Identity is not ClaimsIdentity identity)
                    return Task.FromResult(new ResponseModel<RefreshTokenResponseDTO> { ErrorMessage = "Ошибка получения сущности пользователя" });

                var accessToken = _jwtAuthManager.GenerateToken(identity.Claims, EJwtTokenType.Access);

                return Task.FromResult(new ResponseModel<RefreshTokenResponseDTO>
                {
                    Data = new RefreshTokenResponseDTO
                    {
                        AccessToken = accessToken,
                        Id = identity.Claims.Last().Value,
                        Login = identity.Name,
                    }
                });
            }
            catch (Exception ex)
            {
                var errorMessage = "Ошибка обновления токена";
                _logger.Error(ex, errorMessage);
                return Task.FromResult(new ResponseModel<RefreshTokenResponseDTO> { ErrorMessage = errorMessage });
            }
        }
    }
}
