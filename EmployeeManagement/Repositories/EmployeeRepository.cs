using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Repositories
{
    public class EmployeeRepository : EfRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(AppDbContext db) : base(db) { }

        public async Task<List<Employee>> GetAllWithDepartmentAsync(CancellationToken ct = default)
            => await _db.Employees.Include(e => e.Department).AsNoTracking().ToListAsync(ct);

        public async Task<Employee?> GetByIdWithDepartmentAsync(int id, CancellationToken ct = default)
            => await _db.Employees.Include(e => e.Department).FirstOrDefaultAsync(e => e.Id == id, ct);

        public async Task<List<Employee>> GetByDepartmentNameAsync(string departmentName, CancellationToken ct = default)
            => await _db.Employees
                .Include(e => e.Department)
                .Where(e => e.Department != null && e.Department.Name == departmentName)
                .AsNoTracking()
                .ToListAsync(ct);

        public async Task<List<(string Department, int EmployeeCount)>> GetEmployeeCountsByDepartmentAsync(CancellationToken ct = default)
            => await _db.Employees
                .Include(e => e.Department)
                .AsNoTracking()
                .GroupBy(e => e.Department!.Name)
                .Select(g => new ValueTuple<string, int>(g.Key, g.Count()))
                .ToListAsync(ct);

        public async Task<List<(string Project, decimal TotalSalary)>> GetProjectPayrollsAsync(CancellationToken ct = default)
            => await _db.EmployeeProjects
                .AsNoTracking()
                .Include(ep => ep.Employee)
                .Include(ep => ep.Project)
                .GroupBy(ep => ep.Project!.Name)
                .Select(g => new ValueTuple<string, decimal>(g.Key, g.Sum(x => x.Employee!.Salary)))
                .ToListAsync(ct);

        public async Task<List<(Employee Emp, int ProjectCount)>> GetEmployeesWithProjectCountsAsync(CancellationToken ct = default)
            => await _db.Employees
                .AsNoTracking()
                .GroupJoin(
                    _db.EmployeeProjects.AsNoTracking(),
                    e => e.Id,
                    ep => ep.EmployeeId,
                    (e, eps) => new { Emp = e, ProjectCount = eps.Count() }
                )
                .Select(x => new ValueTuple<Employee, int>(x.Emp, x.ProjectCount))
                .ToListAsync(ct);

        public async Task<List<Employee>> GetTopEarnersPerDepartmentAsync(int topN, CancellationToken ct = default)
        {
            if (topN <= 0) topN = 1;
            var query =
                _db.Employees
                   .Where(e =>
                       _db.Employees.Count(e2 =>
                           e2.DepartmentId == e.DepartmentId &&
                           e2.Salary > e.Salary) < topN)
                   .Include(e => e.Department)                 
                   .AsNoTracking()
                   .OrderBy(e => e.Department!.Name)
                   .ThenByDescending(e => e.Salary);

            return await query.ToListAsync(ct);
        }

        public async Task<List<Employee>> GetEmployeesInHighPayDepartmentsAsync(decimal avgSalaryThreshold, CancellationToken ct = default)
        {
            var richDeptsQuery =
                _db.Employees
                   .Include(e => e.Department)
                   .GroupBy(e => e.Department!.Name)
                   .Where(g => g.Average(e => e.Salary) > avgSalaryThreshold)
                   .Select(g => g.Key);

            return await _db.Employees
                .Include(e => e.Department)
                .Where(e => e.Department != null && richDeptsQuery.Contains(e.Department.Name))
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<List<(string Department, decimal AvgSalary, int Count)>> GetDeptAveragesAsync(CancellationToken ct = default)
            => await _db.Employees
                .Include(e => e.Department)
                .AsNoTracking()
                .GroupBy(e => e.Department!.Name)
                .Select(g => new ValueTuple<string, decimal, int>(
                    g.Key,
                    g.Average(e => e.Salary),
                    g.Count()))
                .ToListAsync(ct);

        public async Task<List<Employee>> GetEmployeesOnProjectAsync(int projectId, CancellationToken ct = default)
        {
            return await _db.Employees
                .Where(e => e.EmployeeProjects.Any(ep => ep.ProjectId == projectId))
                .Include(e => e.Department)          
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public new async Task UpdateAsync(Employee e, CancellationToken ct = default)
        => await base.UpdateAsync(e, ct);

        public new async Task DeleteAsync(Employee e, CancellationToken ct = default)
            => await base.RemoveAsync(e, ct);
    }
}
