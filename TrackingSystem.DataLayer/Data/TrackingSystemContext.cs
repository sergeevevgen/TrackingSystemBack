using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackingSystem.DataLayer.Models;

namespace TrackingSystem.DataLayer.Data
{
    public class TrackingSystemContext : DbContext
    {
        public TrackingSystemContext(DbContextOptions<TrackingSystemContext> options)
        : base(options)
        {
            Database.EnsureCreated();
        }

        public virtual DbSet<Discipline> Disciplines { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Group> Groups { get; set; }

        public virtual DbSet<Role> Roles { get; set; }

        public virtual DbSet<Place> Places { get; set; }

        public virtual DbSet<Subject> Subjects { get; set; }

        public virtual DbSet<User_Role> User_Roles { get; set; }

        public virtual DbSet<User_Subject> User_Subjects { get; set; }
    }
}
