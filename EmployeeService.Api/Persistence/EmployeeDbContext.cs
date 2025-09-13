using EmployeeService.Api.Domain.Entities;
using EmployeeService.Api.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService.Api.Persistence
{
    public class EmployeeDbContext : DbContext
    {
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options) { }

        public DbSet<Employee> Employees => Set<Employee>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(e =>
            {
                e.ToTable("Employees");
                e.HasKey(x => x.Id);
                e.Property(x => x.Name)
                    .HasConversion(n => n.Value, v => new EmployeeName(v))
                    .IsRequired()
                    .HasMaxLength(50);
                e.Property(x => x.Salary).HasPrecision(18, 2);
            });
        }
    }
}