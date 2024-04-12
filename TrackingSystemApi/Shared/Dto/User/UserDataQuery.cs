using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingSystem.Api.Shared.Dto.User
{
    /// <summary>
    /// Запрос данных о пользователе
    /// </summary>
    public class UserDataQuery
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid? Id { get; init; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string? Login { get; init; }

        /// <summary>
        /// Пароль
        /// </summary>
        public string? Password { get; init; }
    }
}
