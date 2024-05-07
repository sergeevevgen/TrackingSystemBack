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
    /// Помещение
    /// </summary>
    [Table("UPlace")]
    public class Place
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
        /// Занятия, проводимые в этом помещении
        /// </summary>
        //[InverseProperty("PlaceId")]
        public virtual ICollection<Subject> Subjects { get; set; }
    }
}
