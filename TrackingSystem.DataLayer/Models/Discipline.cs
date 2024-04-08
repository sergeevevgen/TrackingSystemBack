﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingSystem.DataLayer.Models
{
    public class Discipline
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [InverseProperty("DisciplineId")]
        public virtual ICollection<Subject> Subjects { get; set; }
    }
}
