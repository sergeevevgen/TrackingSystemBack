using TrackingSystem.Api.Shared.Dto.User;
using TrackingSystem.Api.Shared.SharedModels;

namespace TrackingSystem.Api.Shared.IManagers
{
    public interface IUserManager
    {
        /// <summary>
        /// Получить данные о пользователе по контексту
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <returns></returns>
        Task<UserResponseData> GetCurrentUserDataAsync(IHttpContextAccessor httpContextAccessor);

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
        Task<ResponseModel<UserLoginResponseModel>> UserLoginAsync(UserLoginQuery query, CancellationToken cancellationToken);

        /// <summary>
        /// Получение пользователя по идентификатору
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ResponseModel<UserByIdResponse>> FindUserById(UserByIdQuery request, CancellationToken cancellationToken);
    }
}
