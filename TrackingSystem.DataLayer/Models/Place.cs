using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingSystem.DataLayer.Models
{
    public class Place
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
