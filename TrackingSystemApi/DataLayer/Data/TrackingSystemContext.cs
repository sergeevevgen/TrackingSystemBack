using Microsoft.EntityFrameworkCore;
using TrackingSystem.Api.DataLayer.Models;

namespace TrackingSystem.Api.DataLayer.Data
{
    public partial class TrackingSystemContext : DbContext
    {
        public TrackingSystemContext(DbContextOptions<TrackingSystemContext> options)
        : base(options)
        {
            //Database.EnsureCreated();
        }

        public virtual DbSet<Lesson>? Lessons { get; set; }

        public virtual DbSet<User>? Users { get; set; }

        public virtual DbSet<Group>? Groups { get; set; }

        public virtual DbSet<Place>? Places { get; set; }

        public virtual DbSet<Subject>? Subjects { get; set; }

        public virtual DbSet<UserSubject>? UserSubjects { get; set; }

        public virtual DbSet<Info>? Infos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Todo: Надо прописать все типы удалений при удалении связанных сущностей
            modelBuilder.Entity<UserSubject>()
                .HasOne(us => us.User)
                .WithMany(u => u.Subjects)
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Каскадное удаление только для пользователя

            // Внешний ключ для Subject без каскадного удаления
            modelBuilder.Entity<UserSubject>()
                .HasOne(us => us.Subject)
                .WithMany(s => s.Users)
                .HasForeignKey(us => us.SubjectId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
