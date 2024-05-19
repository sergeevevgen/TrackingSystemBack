using Microsoft.Extensions.Options;
using System.Security.Authentication;
using System.Security.Claims;
using TrackingSystem.Api.AppLogic.Core;
using TrackingSystem.Api.Shared.Dto.Group;
using TrackingSystem.Api.Shared.Dto.Identity;
using TrackingSystem.Api.Shared.Dto.Subject;
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
        private readonly IUserDbManager _storage;
        private readonly IJWTAuthManager _jwtManager;
        private readonly IIdentityManager _identityManager;
        private readonly IGroupDbManager _groupManager;
        private readonly ILdapAuthManager _ldapManager;
        private readonly ISubjectDbManager _subjectDbManager;
        private readonly AppConfig _config;

        public UserManager(
            ILogger logger,
            IUserDbManager manager,
            IJWTAuthManager jwtManager,
            IIdentityManager identityManager,
            IGroupDbManager groupManager,
            ILdapAuthManager ldapManager,
            ISubjectDbManager subjectDbManager,
            IOptions<AppConfig> options)
        {
            _logger = logger;
            _storage = manager;
            _jwtManager = jwtManager;
            _identityManager = identityManager;
            _groupManager = groupManager;
            _ldapManager = ldapManager;
            _subjectDbManager = subjectDbManager;
            _config = options.Value;
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

                var userData = await _storage.FindUser(new UserFindDto
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
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Проверяем подключение к LDAP, пожтому на пароли пофик
                var user = await _storage.FindUser(
                    new UserFindDto
                    {
                        Login = query.Login
                    },
                    cancellationToken);

                if (user == null)
                {
                    return new ResponseModel<UserLoginResponseDto> { ErrorMessage = "Неправильный логин/пароль" };
                }

                bool result = false;

                if (user.Role == ERoles.Admin && query.Password.Equals(_config.AdminPassword))
                {
                    result = true;
                }
                else
                {
                    result = true; // раскомментить и убрать
                    //result = _ldapManager.CanAuthorize(query);
                }   

                if (!result)
                    return new ResponseModel<UserLoginResponseDto> { ErrorMessage = "Неправильный логин/пароль" };

                var identity = _identityManager.CreateIdentity(
                    new CreateIdentityDto
                    {
                        Login = user.Login,
                        UserId = user.Id,
                        Role = user.Role
                    },
                    cancellationToken);

                var accessToken = _jwtManager.GenerateToken(
                    identity.Claims,
                    EJwtTokenType.Access);

                var refreshToken = _jwtManager.GenerateToken(
                    identity.Claims,
                    EJwtTokenType.Refresh);

                // Тут надо посмотреть в каком Claims лежит Role
                var response = new ResponseModel<UserLoginResponseDto>
                {
                    Data = new UserLoginResponseDto
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        Name = identity.Name,
                        Id = identity.Claims.Last().Value,
                        Role = identity.Claims.ElementAt(1).Value,
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
        public async Task<ResponseModel<UserFindResponseDto>> FindUserById(UserFindDto request, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _storage.FindUserById(request, cancellationToken);

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

        public async Task<UserResponseDto> CreateOrUpdate(UserDto model, CancellationToken cancellationToken = default)
        {
            var element = await _storage.GetElement(new UserDto
            {
                Login = model.Login
            }, cancellationToken);

            if (element != null && element.Id != model.Id)
            {
                _logger.Error("Уже есть пользователь с такими логином");
                throw new Exception("Уже есть пользователь с такими логином");
            }
 
            if (model.Id.HasValue)
            {
                element = await _storage.Update(model, cancellationToken);
                _logger.Info($"Пользователь с идентификатором {model.Id} обновлен");
            }
            else
            {
                element = await _storage.Insert(model, cancellationToken);
                _logger.Info($"Создан новый пользователь");
            }

            return element;
        }

        public async Task<UserResponseDto> CreateOrUpdateFromLdap(UserLdapDto model, CancellationToken cancellationToken = default)
        {
            // Проверяем, есть ли такой пользователь
            var element = await _storage.GetElement(new UserDto
            {
                Login = model.UserLogin
            }, cancellationToken);

            // Получаем или создаем группу
            var groupId = await GetOrCreateGroupId(model.Group, cancellationToken);

            // Создаем пользователя для вставки или обновления в БД
            var user = new UserDto
            {
                Id = element?.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                MiddleName = model.MiddleName,
                Login = model.UserLogin,
                Status = model.Status ?? (string.IsNullOrEmpty(model.Group) ? null : EStatus.Is_Dropped),
                Role = model.Role,
                GroupId = groupId
            };

            if (element == null)
            {
                return await _storage.Insert(user, cancellationToken);
            }
            else
            {
                return await _storage.Update(user, cancellationToken);
            }
        }

        /// <summary>
        /// Метод для создание или получения группы
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<Guid?> GetOrCreateGroupId(string groupName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                return null;
            }

            var group = await _groupManager.GetElement(new GroupDto { Name = groupName }, cancellationToken);
            if (group != null)
            {
                return group.Id;
            }

            var newGroup = await _groupManager.Insert(new GroupDto { Name = groupName }, cancellationToken);

            return newGroup.Id;
        }

        public async Task<bool> Delete(UserDto model, CancellationToken cancellationToken = default)
        {
            _ = await _storage.GetElement(new UserDto
            {
                Id = model.Id,
            }, cancellationToken) ?? throw new Exception($"Элемент с идентификатором {model.Id} не найден");
            
            await _storage.Delete(model, cancellationToken);
            return true;
        }

        public async Task<ResponseModel<UserResponseDto>> Read(UserDto model, CancellationToken cancellationToken = default)
        {
            if (model != null)
            {
                var data = await _storage.GetElement(model, cancellationToken);
                return new ResponseModel<UserResponseDto> { Data = data };
            }

            return new ResponseModel<UserResponseDto> { ErrorMessage = $"Такой пользователь не найден {model.Login}" };
        }

        public async Task<ResponseModel<bool>> ChangeInfo(InfoChangeDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _subjectDbManager.ChangeInfo(dto);

                if (result)
                {
                    return new ResponseModel<bool> { Data = true };
                }

                return new ResponseModel<bool> { ErrorMessage = "Не удалось обновить инфу" };
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return new ResponseModel<bool> { ErrorMessage = ex.Message };
            }
        }
    }
}
