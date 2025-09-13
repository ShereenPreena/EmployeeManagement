using DepartmentService.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DepartmentService.Api.Data
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
                d.Property(x => x.Id).ValueGeneratedOnAdd();   
                d.Property(x => x.Name).IsRequired().HasMaxLength(100);
                d.HasIndex(x => x.Name).IsUnique();
            });
        }
    }
}
