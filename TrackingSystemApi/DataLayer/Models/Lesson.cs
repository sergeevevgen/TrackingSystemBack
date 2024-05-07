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
    /// Тип занятия
    /// </summary>
    public class Lesson
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
        /// Занятия, проходящие в данной аудитории
        /// </summary>
        //[InverseProperty("DisciplineId")]
        public virtual ICollection<Subject> Subjects { get; set; }
    }
}
