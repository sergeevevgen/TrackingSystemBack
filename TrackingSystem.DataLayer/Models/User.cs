using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackingSystem.Dto.Enums;

namespace TrackingSystem.DataLayer.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        [Required]
        [StringLength(450)]
        public string Login { get; set; }

        [Required]
        [StringLength(450)]
        public string Password { get; set; }

        [ForeignKey("GroupId")]
        public Guid GroupId { get; set; }
        public virtual Group UserGroup { get; set; }

        [Required]
        public EStatus Status { get; set; }

        [InverseProperty("UserId")]
        public virtual ICollection<User_Role> Roles { get; set; }

        [InverseProperty("UserId")]
        public virtual ICollection<User_Subject> Subjects { get; set; }
    }
}
