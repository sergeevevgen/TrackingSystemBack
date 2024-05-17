using TrackingSystem.Api.Shared.Dto.User;
using TrackingSystem.Api.Shared.SharedModels;

namespace TrackingSystem.Api.Shared.IManagers.LogicManagers
{
    public interface IUserManager
    {
        /// <summary>
        /// Получить данные о пользователе по контексту
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <returns></returns>
        Task<UserResponseDto> GetCurrentUserDataAsync(IHttpContextAccessor httpContextAccessor);

        /// <summary>
        /// Определить Id залогиненного пользователя по контексту авторизации
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <returns></returns>
        Guid GetCurrentUserIdByContext(IHttpContextAccessor httpContextAccessor);

        /// <summary>
        /// Вход в аккаунт пользователем
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ResponseModel<UserLoginResponseDto>> UserLoginAsync(UserLoginDto query, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получение пользователя по идентификатору
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ResponseModel<UserFindResponseDto>> FindUserById(UserFindDto request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Метод для создания пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<UserResponseDto> CreateOrUpdate(UserDto model, CancellationToken cancellationToken = default);

        /// <summary>
        /// Метод для удаления пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> Delete(UserDto model, CancellationToken cancellationToken = default);

        /// <summary>
        /// Метод для получения пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ResponseModel<UserResponseDto>> Read(UserDto model, CancellationToken cancellationToken = default);

        /// <summary>
        /// Метод для обновления или создания пользователя из Ldap
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<UserResponseDto> CreateOrUpdateFromLdap(UserLdapDto model, CancellationToken cancellationToken = default);
    }
}
