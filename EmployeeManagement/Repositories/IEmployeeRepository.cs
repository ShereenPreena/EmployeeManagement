
using EmployeeManagement.Models;

namespace EmployeeManagement.Repositories
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<List<Employee>> GetAllWithDepartmentAsync(CancellationToken ct = default);
        Task<Employee?> GetByIdWithDepartmentAsync(int id, CancellationToken ct = default);
        Task<List<Employee>> GetByDepartmentNameAsync(string departmentName, CancellationToken ct = default);
        Task<List<(string Department, int EmployeeCount)>> GetEmployeeCountsByDepartmentAsync(CancellationToken ct = default);               
        Task<List<(string Project, decimal TotalSalary)>> GetProjectPayrollsAsync(CancellationToken ct = default);                           
        Task<List<(Employee Emp, int ProjectCount)>> GetEmployeesWithProjectCountsAsync(CancellationToken ct = default);                 
        Task<List<Employee>> GetTopEarnersPerDepartmentAsync(int topN, CancellationToken ct = default);                                     
        Task<List<Employee>> GetEmployeesInHighPayDepartmentsAsync(decimal avgSalaryThreshold, CancellationToken ct = default);              
        Task<List<(string Department, decimal AvgSalary, int Count)>> GetDeptAveragesAsync(CancellationToken ct = default);                   
        Task<List<Employee>> GetEmployeesOnProjectAsync(int projectId, CancellationToken ct = default);
        Task DeleteAsync(Employee e, CancellationToken ct = default);
    }
}
