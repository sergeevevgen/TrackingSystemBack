using NLog;
using System.Security.Authentication;
using System.Security.Claims;
using TrackingSystem.Api.DataLayer.DataAccessManagers;
using TrackingSystem.Api.Shared.Dto.Identity;
using TrackingSystem.Api.Shared.Dto.User;
using TrackingSystem.Api.Shared.Enums;
using TrackingSystem.Api.Shared.IManagers;
using TrackingSystem.Api.Shared.SharedModels;
using ILogger = NLog.ILogger;

namespace TrackingSystem.Api.BusinessLogic.Managers
{
    /// <summary>
    /// Менеджер для взаимодействия с пользователем
    /// </summary>
    public class UserManager : IUserManager
    {
        private readonly ILogger _logger;
        private readonly UserDbManager _manager;
        private readonly IJWTAuthManager _jwtManager;
        private readonly IdentityManager _identityManager;

        public UserManager(
            ILogger logger,
            UserDbManager manager,
            IJWTAuthManager jwtManager,
            IdentityManager identityManager)
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
        public Task<UserResponseData> GetCurrentUserDataAsync(IHttpContextAccessor httpContextAccessor)
        {
            try
            {
                var userId = GetCurrentUserIdByContext(httpContextAccessor);

                //var userData = await _mediator.Send(new UserDataQuery
                //{
                //    Id = userId,
                //});

                return null;
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
        public async Task<ResponseModel<UserLoginResponseModel>> UserLoginAsync(
            UserLoginQuery query,
            CancellationToken cancellationToken)
        {
            try
            {
                var user = await _manager.FindUser(
                    new UserDataQuery
                    {
                        Login = query.Login,
                        Password = query.Password,
                    },
                    cancellationToken);

                if (user == null)
                    return new ResponseModel<UserLoginResponseModel> { ErrorMessage = "Неправильный логин / пароль" };

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

                var response = new ResponseModel<UserLoginResponseModel>
                {
                    Data = new UserLoginResponseModel
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
                return new ResponseModel<UserLoginResponseModel> { ErrorMessage = errorMessage };
            }
        }

        /// <summary>
        /// Получение пользователя (только имя и email) по его Id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ResponseModel<UserByIdResponse>> FindUserById(UserByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _manager.FindUserById(request, cancellationToken);

                if (result == null)
                    return new ResponseModel<UserByIdResponse>() { ErrorMessage = "Не удалось найти пользователя по Id" };
                else
                    return new ResponseModel<UserByIdResponse>() { Data = result };
            }
            catch (Exception ex)
            {
                var errorMessage = "Не удалось получить пользователя";
                _logger.Error(ex, errorMessage);
                return new ResponseModel<UserByIdResponse>() { ErrorMessage = errorMessage };
            }
        }
    }
}
