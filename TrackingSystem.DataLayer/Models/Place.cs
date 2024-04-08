﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingSystem.DataLayer.Models
{
    public class Place
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [InverseProperty("PlaceId")]
        public virtual ICollection<Subject> Subjects { get; set; }
    }
}
