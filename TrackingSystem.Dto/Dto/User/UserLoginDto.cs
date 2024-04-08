using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingSystem.Shared.Dto.User
{
    /// <summary>
    /// Dto для авторизации
    /// </summary>
    public class UserLoginDto
    {
        /// <summary>
        /// Логин
        /// </summary>
        [Required(ErrorMessage = "Поле Login не может быть пустым")]
        public string Login { get; init; } = null!;

        /// <summary>
        /// Пароль
        /// </summary>
        [Required(ErrorMessage = "Поле Password не может быть пустым")]
        public string Password { get; init; } = null!;
    }
}
