using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackingSystem.Api.Shared.Enums;

namespace TrackingSystem.Api.Shared.Dto.User
{
    /// <summary>
    /// Dto для ответа на запрос авторизации
    /// </summary>
    public class UserLoginResponseDto
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public string Id { get; init; } = null!;

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string? Name { get; init; }

        /// <summary>
        /// Токен доступа
        /// </summary>
        public string AccessToken { get; init; } = null!;

        /// <summary>
        /// Токен обновления
        /// </summary>
        public string RefreshToken { get; init; } = null!;

        /// <summary>
        /// Роль
        /// </summary>
        public List<RoleEnum> Roles { get; init; } = new List<RoleEnum>();
    }
}
