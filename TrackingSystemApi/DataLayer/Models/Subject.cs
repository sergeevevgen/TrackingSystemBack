using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackingSystem.Api.Shared.Enums;

namespace TrackingSystem.Api.DataLayer.Models
{
    public class Subject
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public int Week { get; set; }

        [Required]
        public int Day { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public EPairNumbers Pair { get; set; }

        [ForeignKey("GroupId")]
        public Guid GroupId { get; set; }
        public virtual Group Group { get; set; }

        [ForeignKey("PlaceId")]
        public Guid PlaceId { get; set; }
        public virtual Place Place { get; set; }

        [ForeignKey("DisciplineId")]
        public Guid DisciplineId { get; set; }
        public virtual Discipline Discipline { get; set; }

        //[InverseProperty("SubjectId")]
        public virtual ICollection<User_Subject> Users { get; set; }
    }
}
