using EmployeeService.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService.Api.Data
{
    public class EmployeeDbContext : DbContext
    {
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options) { }

        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<EmployeeProject> EmployeeProjects => Set<EmployeeProject>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(e =>
            {
                e.ToTable("Employees");
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired().HasMaxLength(50);
                e.Property(x => x.Salary).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Project>(p =>
            {
                p.ToTable("Projects");
                p.HasKey(x => x.Id);
                p.Property(x => x.Name).IsRequired().HasMaxLength(100);
                p.Property(x => x.Budget).HasPrecision(18, 2);
            });

            modelBuilder.Entity<EmployeeProject>(ep =>
            {
                ep.ToTable("EmployeeProjects");
                ep.HasKey(x => new { x.EmployeeId, x.ProjectId });
             
                modelBuilder.Entity<EmployeeProject>()
                    .Property(x => x.AssignedAt)
                    .HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}
