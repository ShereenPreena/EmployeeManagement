using DepartmentService.Api.Domain.Entities;
using DepartmentService.Api.Domain.Repositories;
using DepartmentService.Api.Domain.ValueObjects;
using DepartmentService.Api.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DepartmentService.Api.Infrastructure.Repositories
{
    public class EfDepartmentRepository : IDepartmentRepository
    {
        private readonly DepartmentDbContext _db;
        public EfDepartmentRepository(DepartmentDbContext db) { _db = db; }

        public void Add(Department department) => _db.Departments.Add(department);
        public void Remove(Department department) => _db.Departments.Remove(department);

        public async Task<IEnumerable<Department>> GetAllAsync(CancellationToken ct)
            => await _db.Departments.AsNoTracking().ToListAsync(ct);

        public Task<Department?> GetByIdAsync(int id, CancellationToken ct)
            => _db.Departments.FindAsync(new object[] { id }, ct).AsTask();

        public async Task<Department?> GetByNameAsync(DepartmentName name, CancellationToken ct)
            => await _db.Departments.AsNoTracking().FirstOrDefaultAsync(d => d.Name == name, ct);

        public Task<bool> ExistsByNameAsync(DepartmentName name, CancellationToken ct)
            => _db.Departments.AnyAsync(d => d.Name == name, ct);

        public Task<int> SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
    }
}