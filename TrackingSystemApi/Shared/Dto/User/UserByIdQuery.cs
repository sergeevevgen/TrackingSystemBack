using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingSystem.Api.Shared.Dto.User
{
    /// <summary>
    /// Получение пользователя (только имя и email) по его Id
    /// </summary>
    public class UserByIdQuery
    {
        /// <summary>
        /// Id пользователя отправляющего запрос
        /// </summary>
        [Required(ErrorMessage = "Поле UserId не может быть пустым")]
        public Guid UserId { get; set; }
    }
}
