using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingSystem.Api.Shared.Dto.User
{
    /// <summary>
    /// Запрос данных о пользователе
    /// </summary>
    public class UserFindDto
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Required(ErrorMessage = "Поле UserId не может быть пустым")]
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
