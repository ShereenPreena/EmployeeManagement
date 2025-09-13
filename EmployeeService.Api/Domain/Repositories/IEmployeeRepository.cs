using EmployeeService.Api.Domain.Entities;

namespace EmployeeService.Api.Domain.Repositories
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync(CancellationToken ct);
        Task<Employee?> GetByIdAsync(int id, CancellationToken ct);
        void Add(Employee employee);
        void Remove(Employee employee);
        Task<int> SaveChangesAsync(CancellationToken ct);
    }
}