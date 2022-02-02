using Core.DbModel.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.DbModel.Contexts
{
    public class StudentContext : DbContext
    {
        public StudentContext(DbContextOptions options) : base(options)
        { }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Teacher> Teachers { get; set; }
        public virtual DbSet<Class> Classes { get; set; }
        public virtual DbSet<Score> Scores { get; set; }
        public virtual DbSet<Subject> Subjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>().HasData(new Student
            {
                Id = 1,
                Name = "Pablo",
                LastName = "Pereira"
            });
            modelBuilder.Entity<Teacher>().HasData(new Teacher
            {
                Id = 1,
                Name = "Jaldin",
                LastName = "Jimenez"
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            //Configura las navegaciones para que se carguen automaticamente siempre que se las demande
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
