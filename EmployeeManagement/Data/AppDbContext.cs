using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<EmployeeProject> EmployeeProjects => Set<EmployeeProject>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Employee
            modelBuilder.Entity<Employee>(e =>
            {
                e.ToTable("Employees");

                e.HasKey(x => x.Id);

                e.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(50); 

                e.Property(x => x.Salary)
                    .HasPrecision(18, 2);

                e.HasCheckConstraint("CK_Employees_Age_NonNegative", "[Age] >= 0");

                e.HasOne(x => x.Department)
                    .WithMany(d => d.Employees)
                    .HasForeignKey(x => x.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict); 
            });

            // Department
            modelBuilder.Entity<Department>(d =>
            {
                d.ToTable("Departments");

                d.HasKey(x => x.Id);

                d.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                d.HasIndex(x => x.Name)
                    .IsUnique(); // unique department names
            });

            // Project 
            modelBuilder.Entity<Project>(p =>
            {
                p.ToTable("Projects");

                p.HasKey(x => x.Id);

                p.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                p.Property(x => x.Budget)
                    .HasPrecision(18, 2);
            });

            // Many-to-many: Employee <-> Project 
            modelBuilder.Entity<EmployeeProject>(ep =>
            {
                ep.ToTable("EmployeeProjects");

                ep.HasKey(x => new { x.EmployeeId, x.ProjectId }); 

                ep.HasOne(x => x.Employee)
                    .WithMany(e => e.EmployeeProjects)
                    .HasForeignKey(x => x.EmployeeId);

                ep.HasOne(x => x.Project)
                    .WithMany(p => p.EmployeeProjects)
                    .HasForeignKey(x => x.ProjectId);

                ep.Property(x => x.AssignedAt)
                  .HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}
