using EmployeeService.Api.DepartmentClient;
using EmployeeService.Api.Domain.Entities;
using EmployeeService.Api.Domain.Repositories;
using EmployeeService.Api.Domain.ValueObjects;
using EmployeeService.Api.Dto;

namespace EmployeeService.Api.Application
{
    public class EmployeeAppService
    {
        private readonly IEmployeeRepository _repo;
        private readonly IDepartmentClient _dept;

        public EmployeeAppService(IEmployeeRepository repo, IDepartmentClient dept)
        {
            _repo = repo; _dept = dept;
        }

        public async Task<IEnumerable<EmployeeReadDto>> GetAllAsync(CancellationToken ct)
        {
            var list = await _repo.GetAllAsync(ct);
            return list.Select(e => new EmployeeReadDto
            {
                Id = e.Id,
                Name = e.Name,
                Age = e.Age,
                Salary = e.Salary,
                IsPermanent = e.IsPermanent,
                DepartmentId = e.DepartmentId
            });
        }

        public async Task<EmployeeReadDto?> GetByIdAsync(int id, CancellationToken ct)
        {
            var e = await _repo.GetByIdAsync(id, ct);
            return e is null ? null : new EmployeeReadDto
            {
                Id = e.Id,
                Name = e.Name,
                Age = e.Age,
                Salary = e.Salary,
                IsPermanent = e.IsPermanent,
                DepartmentId = e.DepartmentId
            };
        }

        public async Task<EmployeeReadDto?> CreateAsync(EmployeeCreateDto dto, CancellationToken ct)
        {
            if (!await _dept.DepartmentExistsAsync(dto.DepartmentId, ct)) return null;
            var employee = new Employee(new EmployeeName(dto.Name), dto.Age, dto.Salary, dto.IsPermanent, dto.DepartmentId);
            _repo.Add(employee);
            await _repo.SaveChangesAsync(ct);
            return new EmployeeReadDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Age = employee.Age,
                Salary = employee.Salary,
                IsPermanent = employee.IsPermanent,
                DepartmentId = employee.DepartmentId
            };
        }

        public async Task<bool> UpdateAsync(int id, EmployeeUpdateDto dto, CancellationToken ct)
        {
            var e = await _repo.GetByIdAsync(id, ct);
            if (e is null) return false;
            if (!await _dept.DepartmentExistsAsync(dto.DepartmentId, ct)) return false;
            e.Update(new EmployeeName(dto.Name), dto.Age, dto.Salary, dto.IsPermanent, dto.DepartmentId);
            await _repo.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var e = await _repo.GetByIdAsync(id, ct);
            if (e is null) return false;
            _repo.Remove(e);
            await _repo.SaveChangesAsync(ct);
            return true;
        }
    }
}