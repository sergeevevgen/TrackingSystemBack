using Microsoft.AspNetCore.Http;
using TrackingSystem.Shared.Dto.User;

namespace TrackingSystem.Shared.IManagers
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
    }
}
