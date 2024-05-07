using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackingSystem.Api.Shared.Enums;

namespace TrackingSystem.Api.DataLayer.Models
{
    /// <summary>
    /// Роли
    /// </summary>
    public class Role
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Есть два типа роли: Учитель и ученик
        /// </summary>
        [Required]
        public ERoles Name { get; set; }

        /// <summary>
        /// Связанные роли и пользователи
        /// </summary>
        //[InverseProperty("RoleId")]
        public virtual ICollection<UserRole> Users { get; set; }
    }
}
