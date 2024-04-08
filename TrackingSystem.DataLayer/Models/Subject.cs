﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackingSystem.Dto.Enums;

namespace TrackingSystem.DataLayer.Models
{
    public class Subject
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public int Week { get; set; }

        [Required]
        public int Day { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public EPairNumbers Pair { get; set; }

        [ForeignKey("GroupId")]
        public int GroupId { get; set; }
        public virtual Group Group { get; set; }

        [ForeignKey("PlaceId")]
        public int PlaceId { get; set; }
        public virtual Place Place { get; set; }

        [ForeignKey("DisciplineId")]
        public int DisciplineId { get; set; }
        public virtual Discipline Discipline { get; set; }

        [InverseProperty("SubjectId")]
        public virtual ICollection<User_Subject> Users { get; set; }
    }
}
