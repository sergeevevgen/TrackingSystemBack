using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public int WeekNumber { get; set; }
        public int DayNumber { get; set; }
        public string Type { get; set; }
        public PairNumber Pair { get; set; }
    }
}
