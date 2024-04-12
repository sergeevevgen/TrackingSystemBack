using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingSystem.Api.Shared.Dto.User
{
    /// <summary>
    /// Для отображения пользователя для лругих пользователей (только имя и логин)
    /// </summary>
    public class UserByIdResponse
    {
        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string Login { get; set; } = null!;

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name { get; set; } = null!;
    }
}
