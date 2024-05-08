using NLog;
using System.Security.Authentication;
using System.Security.Claims;
using TrackingSystem.Api.DataLayer.DataAccessManagers;
using TrackingSystem.Api.Shared.Dto.Identity;
using TrackingSystem.Api.Shared.Dto.User;
using TrackingSystem.Api.Shared.Enums;
using TrackingSystem.Api.Shared.IManagers;
using TrackingSystem.Api.Shared.IManagers.DbManagers;
using TrackingSystem.Api.Shared.IManagers.LogicManagers;
using TrackingSystem.Api.Shared.SharedModels;

namespace TrackingSystem.Api.BusinessLogic.Managers
{
    /// <summary>
    /// Менеджер для взаимодействия с пользователем
    /// </summary>
    public class UserManager : IUserManager
    {
        private readonly ILogger _logger;
        private readonly IUserDbManager _manager;
        private readonly IJWTAuthManager _jwtManager;
        private readonly IIdentityManager _identityManager;

        public UserManager(
            ILogger logger,
            IUserDbManager manager,
            IJWTAuthManager jwtManager,
            IIdentityManager identityManager)
        {
            _logger = logger;
            _manager = manager;
            _jwtManager = jwtManager;
            _identityManager = identityManager;
        }

        /// <summary>
        /// Метод получения данных пользователя по контексту
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <returns></returns>
        public async Task<UserResponseDto> GetCurrentUserDataAsync(IHttpContextAccessor httpContextAccessor)
        {
            try
            {
                var userId = GetCurrentUserIdByContext(httpContextAccessor);

                var userData = await _manager.FindUser(new UserFindDto
                {
                    Id = userId,
                }, default);

                return userData;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка получения данных о пользователе");
                throw;
            }
        }

        /// <summary>
        /// Метод получения идентификатора пользователя по контексту
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <returns></returns>
        public Guid GetCurrentUserIdByContext(IHttpContextAccessor httpContextAccessor)
        {
            try
            {
                var identity = httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity
                    ?? throw new Exception("Ошибка получения ClaimsIdentity");

                if (!identity.IsAuthenticated)
                    throw new AuthenticationException("Пользователь не вошёл в систему");

                var id = identity.FindFirst("Id")?.Value;
                if (string.IsNullOrEmpty(id))
                    throw new AuthenticationException("Невозможно определить Id пользователя в контексте сервера");

                return Guid.Parse(id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка определения Id пользователя");
                throw;
            }
        }

        /// <summary>
        /// Метод логина
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ResponseModel<UserLoginResponseDto>> UserLoginAsync(
            UserLoginDto query,
            CancellationToken cancellationToken)
        {
            try
            {
                var user = await _manager.FindUser(
                    new UserFindDto
                    {
                        Login = query.Login,
                        Password = query.Password,
                    },
                    cancellationToken);

                if (user == null)
                    return new ResponseModel<UserLoginResponseDto> { ErrorMessage = "Неправильный логин / пароль" };

                var identity = await _identityManager.CreateIdentity(
                    new CreateIdentityCommand
                    {
                        Login = user.Login,
                        UserId = user.Id,
                        // Role = TODO
                    },
                    cancellationToken);

                var accessToken = _jwtManager.GenerateToken(
                    identity.Claims,
                    EJwtTokenType.Access);

                var refreshToken = _jwtManager.GenerateToken(
                    identity.Claims,
                    EJwtTokenType.Refresh);

                var response = new ResponseModel<UserLoginResponseDto>
                {
                    Data = new UserLoginResponseDto
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        Name = identity.Name,
                        Id = identity.Claims.Last().Value,
                        //TODO role
                    }
                };

                return response;
            }
            catch(Exception ex)
            {
                var errorMessage = "Неправильный логин / пароль";
                _logger.Error(ex, errorMessage);
                return new ResponseModel<UserLoginResponseDto> { ErrorMessage = errorMessage };
            }
        }

        /// <summary>
        /// Получение пользователя (только имя и email) по его Id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ResponseModel<UserFindResponseDto>> FindUserById(UserFindDto request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _manager.FindUserById(request, cancellationToken);

                if (result == null)
                    return new ResponseModel<UserFindResponseDto>() { ErrorMessage = "Не удалось найти пользователя по Id" };
                else
                    return new ResponseModel<UserFindResponseDto>() { Data = result };
            }
            catch (Exception ex)
            {
                var errorMessage = "Не удалось получить пользователя";
                _logger.Error(ex, errorMessage);
                return new ResponseModel<UserFindResponseDto>() { ErrorMessage = errorMessage };
            }
        }

        public Task<UserResponseDto> CreateOrUpdate(UserDto model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(UserDto model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<UserResponseDto>> Read(UserDto model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
