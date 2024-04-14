using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackingSystem.Api.Shared.Enums;

namespace TrackingSystem.Api.DataLayer.Models
{
    public class Subject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public int Week { get; set; }

        [Required]
        public int Day { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public EPairNumbers Pair { get; set; }

        [Required]
        public int IsDifference { get; set; }

        [ForeignKey("GroupId")]
        public Guid GroupId { get; set; }
        public virtual Group Group { get; set; }

        [ForeignKey("PlaceId")]
        public Guid PlaceId { get; set; }
        public virtual Place Place { get; set; }

        [ForeignKey("LessonId")]
        public Guid LessonId { get; set; }
        public virtual Lesson Lesson { get; set; }

        //[InverseProperty("SubjectId")]
        public virtual ICollection<User_Subject> Users { get; set; }
    }
}
