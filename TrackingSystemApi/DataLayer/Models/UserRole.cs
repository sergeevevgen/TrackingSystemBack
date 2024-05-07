using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingSystem.Api.DataLayer.Models
{
    /// <summary>
    /// Таблица для связи пользователя и роли
    /// </summary>
    public class UserRole
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        [ForeignKey("UserId")]
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        [ForeignKey("RoleId")]
        public Guid RoleId { get; set; }
        public virtual Role Role { get; set; }
    }
}
