using DepartmentService.Api.Domain.Entities;
using DepartmentService.Api.Domain.ValueObjects;

namespace DepartmentService.Api.Domain.Repositories
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllAsync(CancellationToken ct);
        Task<Department?> GetByIdAsync(int id, CancellationToken ct);
        Task<Department?> GetByNameAsync(DepartmentName name, CancellationToken ct);
        Task<bool> ExistsByNameAsync(DepartmentName name, CancellationToken ct);
        void Add(Department department);
        void Remove(Department department);
        Task<int> SaveChangesAsync(CancellationToken ct);
    }
}