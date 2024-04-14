using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingSystem.Api.DataLayer.Models
{
    public class User_Subject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public bool IsMarked { get; set; }

        [Required]
        public DateTime MarkTime { get; set; }

        [ForeignKey("UserId")]
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey("SubjectId")]
        public Guid SubjectId { get; set; }
        public virtual Subject Subject { get; set; }
    }
}
