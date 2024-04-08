using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingSystem.DataLayer.Data
{
    public class TrackingSystemContext : DbContext
    {
        public TrackingSystemContext(DbContextOptions<TrackingSystemContext> options)
        : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
