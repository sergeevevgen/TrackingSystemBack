using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackingSystem.Api.Shared.Enums;

namespace TrackingSystem.Api.Shared.Dto.Identity
{
    /// <summary>
    /// Создание сущности Identity пользователя
    /// по логину и паролю
    /// </summary>
    public class CreateIdentityDto
    {
        /// <summary>
        /// Id пользователя
        /// </summary>
        [Required(ErrorMessage = "Поле UserId должно быть заполнено")]
        public Guid UserId { get; init; }

        /// <summary>
        /// Логин
        /// </summary>
        [Required(ErrorMessage = "Поле Login должно быть заполнено")]
        public string Login { get; init; } = null!;

        /// <summary>
        /// Роль
        /// </summary>
        public List<RoleEnum> Roles { get; init; } = new List<RoleEnum>();
    }
}
