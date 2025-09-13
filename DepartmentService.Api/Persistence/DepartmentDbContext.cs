using DepartmentService.Api.Domain.Entities;
using DepartmentService.Api.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DepartmentService.Api.Persistence
{
    public class DepartmentDbContext : DbContext
    {
        public DepartmentDbContext(DbContextOptions<DepartmentDbContext> options) : base(options) { }

        public DbSet<Department> Departments => Set<Department>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>(d =>
            {
                d.ToTable("Departments");
                d.HasKey(x => x.Id);
                d.Property(x => x.Name)
                    .HasConversion(n => n.Value, v => new DepartmentName(v))
                    .IsRequired()
                    .HasMaxLength(100);
                d.HasIndex(x => x.Name).IsUnique();
            });
        }
    }
}