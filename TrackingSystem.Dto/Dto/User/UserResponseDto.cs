using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackingSystem.Shared.Enums;

namespace TrackingSystem.Shared.Dto.User
{
    /// <summary>
    /// Dto для ответа на запрос о получении данных пользователя
    /// </summary>
    public class UserResponseDto
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Логин
        /// </summary>
        public string Login { get; set; } = null!;

        /// <summary>
        /// Роли
        /// </summary>
        public List<ERoles>? Roles { get; set; }

        /// <summary>
        /// ФИО пользователя
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Учебная группа
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Статус (отчислен, вышел из академа, учится)
        /// </summary>
        public EStatus? Status { get; set; }
    }
}
