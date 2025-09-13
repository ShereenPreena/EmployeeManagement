using EmployeeService.Api.Domain.Entities;
using EmployeeService.Api.Domain.Repositories;
using EmployeeService.Api.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService.Api.Infrastructure.Repositories
{
    public class EfEmployeeRepository : IEmployeeRepository
    {
        private readonly EmployeeDbContext _db;
        public EfEmployeeRepository(EmployeeDbContext db) { _db = db; }

        public void Add(Employee employee) => _db.Employees.Add(employee);
        public void Remove(Employee employee) => _db.Employees.Remove(employee);

        public async Task<IEnumerable<Employee>> GetAllAsync(CancellationToken ct)
            => await _db.Employees.AsNoTracking().ToListAsync(ct);

        public Task<Employee?> GetByIdAsync(int id, CancellationToken ct)
            => _db.Employees.FindAsync(new object[] { id }, ct).AsTask();

        public Task<int> SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
    }
}