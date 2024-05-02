using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingSystem.Api.DataLayer.Models
{
    public class Group
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        //[InverseProperty("GroupId")]
        public virtual ICollection<User>? Users { get; set; }

        //[InverseProperty("SubjectId")]
        public virtual ICollection<Subject>? Subjects { get; set; }
    }
}
