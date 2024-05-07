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
    /// Класс для связи занятия и пользователя. Используется для отметки посещения
    /// </summary>
    [Table("UUserSubject")]
    public class UserSubject
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Флаг отметки
        /// </summary>
        [Required]
        public bool IsMarked { get; set; }

        /// <summary>
        /// Время отметки
        /// </summary>
        [Required]
        public DateTime MarkTime { get; set; }

        /// <summary>
        /// Пользователь. Может не существовать, если его удалить
        /// </summary>
        [ForeignKey("UserId")]
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        /// <summary>
        /// Занятие
        /// </summary>
        [ForeignKey("SubjectId")]
        public Guid SubjectId { get; set; }
        public virtual Subject Subject { get; set; }
    }
}
