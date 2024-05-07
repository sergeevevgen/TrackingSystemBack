using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackingSystem.Api.Shared.Enums;

namespace TrackingSystem.Api.DataLayer.Models
{
    /// <summary>
    /// Пользователь - ученик или учитель
    /// </summary>
    [Table("UUser")]
    public class User
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// ФИО
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Логин
        /// </summary>
        [Required]
        [StringLength(450)]
        public string Login { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        [Required]
        [StringLength(450)]
        public string Password { get; set; }

        /// <summary>
        /// Внешний ключ к группе
        /// </summary>
        [ForeignKey("GroupId")]
        public Guid? GroupId { get; set; }
        public virtual Group? UserGroup { get; set; }

        /// <summary>
        /// Статус пользователя в системе
        /// </summary>
        [Required]
        public EStatus Status { get; set; }

        /// <summary>
        /// Связь один-к-одному для учителя и занятия
        /// </summary>
        public virtual Subject Subject { get; set; } = null!;

        /// <summary>
        /// Связь с ролями
        /// </summary>
        //[InverseProperty("UserId")]
        public virtual ICollection<UserRole>? Roles { get; set; }

        /// <summary>
        /// Связь с занятиями для проставления отметок
        /// </summary>
        //[InverseProperty("UserId")]
        public virtual ICollection<UserSubject>? Subjects { get; set; }
    }
}
