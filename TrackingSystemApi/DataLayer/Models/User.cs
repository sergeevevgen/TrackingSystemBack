using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackingSystem.Api.Shared.Enums;

namespace TrackingSystem.Api.DataLayer.Models
{
    /// <summary>
    /// Пользователь - ученик или учитель
    /// </summary>
    public class User
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        [Required]
        public string LastName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        [Required]
        public string MiddleName { get; set; }

        /// <summary>
        /// Логин
        /// </summary>
        [Required]
        [StringLength(450)]
        public string Login { get; set; }

        /// <summary>
        /// Внешний ключ к группе
        /// </summary>
        [ForeignKey("GroupId")]
        public Guid? GroupId { get; set; }
        public virtual Group? UserGroup { get; set; }

        /// <summary>
        /// Статус пользователя в системе
        /// </summary>
        public Status? Status { get; set; }

        /// <summary>
        /// Связь один-ко-многим для учителя и занятия
        /// </summary>
        public virtual ICollection<Subject>? TeacherSubjects { get; set; } = null!;

        /// <summary>
        /// Для связи многие-ко-многим пользователи и роли
        /// </summary>
        public ICollection<UserRole> UserRoles { get; set; }

        /// <summary>
        /// Связь с занятиями для проставления отметок
        /// </summary>
        //[InverseProperty("UserId")]
        public virtual ICollection<UserSubject>? Subjects { get; set; }
    }
}
