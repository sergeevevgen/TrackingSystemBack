using Microsoft.EntityFrameworkCore;
using TrackingSystem.Api.DataLayer.Models;

namespace TrackingSystem.Api.DataLayer.Data
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User_Subject>()
                .HasOne(us => us.User)
                .WithMany(u => u.Subjects)
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Каскадное удаление только для пользователя

            // Внешний ключ для Subject без каскадного удаления
            modelBuilder.Entity<User_Subject>()
                .HasOne(us => us.Subject)
                .WithMany(s => s.Users)
                .HasForeignKey(us => us.SubjectId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
