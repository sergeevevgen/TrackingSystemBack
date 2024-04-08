using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingSystem.DataLayer.Models
{
    public class User_Subject
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid SubjectId { get; set; }
        public bool IsMarked { get; set; }
        public DateTime MarkTime { get; set; }
    }
}
