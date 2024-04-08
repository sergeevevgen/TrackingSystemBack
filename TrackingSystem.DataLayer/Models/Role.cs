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
    public class Role
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public ERoles Name { get; set; }

        [InverseProperty("RoleId")]
        public virtual ICollection<User_Role> Users { get; set; }
    }
}
