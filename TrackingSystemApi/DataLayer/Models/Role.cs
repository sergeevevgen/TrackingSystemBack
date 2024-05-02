using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackingSystem.Api.Shared.Enums;

namespace TrackingSystem.Api.DataLayer.Models
{
    public class Role
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public ERoles Name { get; set; }

        //[InverseProperty("RoleId")]
        public virtual ICollection<UserRole> Users { get; set; }
    }
}
