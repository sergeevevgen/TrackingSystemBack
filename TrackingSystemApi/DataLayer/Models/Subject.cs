using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackingSystem.Api.Shared.Enums;

namespace TrackingSystem.Api.DataLayer.Models
{
    /// <summary>
    /// Сама сущность занятия
    /// </summary>
    [Table("USubject")]
    public class Subject
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Номер недели
        /// </summary>
        [Required]
        public int Week { get; set; }

        /// <summary>
        /// Номер дня (пн, вт, ср, чт, пт, сб, вск - возможно, стоит добавить перечисление?)
        /// </summary>
        [Required]
        public int Day { get; set; }

        /// <summary>
        /// Тип занятия
        /// </summary>
        [Required]
        public string? Type { get; set; }

        /// <summary>
        /// Номер пары в расписании
        /// </summary>
        [Required]
        public EPairNumbers Pair { get; set; }

        /// <summary>
        /// Было ли изменено данное занятие или нет?
        /// </summary>
        [Required]
        public int IsDifference { get; set; }

        /// <summary>
        /// Внешний ключ к группе
        /// </summary>
        [ForeignKey("GroupId")]
        public Guid GroupId { get; set; }
        public virtual Group Group { get; set; } = null!;

        /// <summary>
        /// Внешний ключ к помещению
        /// </summary>
        [ForeignKey("PlaceId")]
        public Guid PlaceId { get; set; }
        public virtual Place Place { get; set; } = null!;

        /// <summary>
        /// Внешний ключ к типу занятия
        /// </summary>
        [ForeignKey("LessonId")]
        public Guid LessonId { get; set; }
        public virtual Lesson Lesson { get; set; } = null!;

        /// <summary>
        /// Внешний ключ к учителю
        /// </summary>
        [ForeignKey("TeacherId")]
        public Guid TeacherId { get; set; }
        public virtual User Teacher { get; set; } = null!;

        /// <summary>
        /// Так связываются пользователи для отметки посещения
        /// </summary>
        //[InverseProperty("SubjectId")]
        public virtual ICollection<UserSubject>? Users { get; set; }
    }
}