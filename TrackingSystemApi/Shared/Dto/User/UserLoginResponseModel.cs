using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingSystem.Api.Shared.Dto.User
{
    /// <summary>
    /// Dto для ответа на запрос авторизации
    /// </summary>
    public class UserLoginResponseModel
    {
        /// <summary>
        /// Токен доступа
        /// </summary>
        public string AccessToken { get; init; } = null!;

        /// <summary>
        /// Токен обновления
        /// </summary>
        public string RefreshToken { get; init; } = null!;

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public string Id { get; init; } = null!;
    }
}
