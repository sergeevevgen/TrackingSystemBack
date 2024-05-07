using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingSystem.Api.DataLayer.Models
{
    /// <summary>
    /// Группа
    /// </summary>
    [Table("UGroup")]
    public class Group
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Ученики, связанные с группой
        /// </summary>
        //[InverseProperty("GroupId")]
        public virtual ICollection<User>? Users { get; set; }

        /// <summary>
        /// Занятия, связанные с группой    
        /// </summary>
        //[InverseProperty("SubjectId")]
        public virtual ICollection<Subject>? Subjects { get; set; }
    }
}
