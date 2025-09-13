using EmployeeManagement.Interfaces;
using EmployeeManagement.Models;
using EmployeeManagement.Data;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Repositories;

namespace EmployeeManagement.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _db;                  
        private readonly IEmployeeRepository _repo;         

        public EmployeeService(AppDbContext db, IEmployeeRepository repo)
        {
            _db = db;
            _repo = repo;
        }

        public async Task<Department> EnsureDepartmentAsync(string name)
        {
            var normalized = (name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(normalized))
                throw new ArgumentException("Department name is required", nameof(name));

            var dept = await _db.Departments.FirstOrDefaultAsync(d => d.Name == normalized);
            if (dept != null) return dept;

            dept = new Department { Name = normalized };
            _db.Departments.Add(dept);
            await _db.SaveChangesAsync();
            return dept;
        }

        public Task AddEmployeeAsync(Employee employee) => _repo.AddAsync(employee);

        public async Task UpdateEmployeeAsync(Employee employee)
        => await _repo.UpdateAsync(employee);

        public async Task DeleteEmployeeAsync(int id)
        {
            var existing = await _db.Employees.FindAsync(id);
            if (existing != null)
                await _repo.DeleteAsync(existing);
        }

        public Task<Employee?> GetEmployeeByIdAsync(int id)
            => _repo.GetByIdWithDepartmentAsync(id);

        public Task<List<Employee>> GetAllEmployeesAsync()
            => _repo.GetAllWithDepartmentAsync();
    }
}
