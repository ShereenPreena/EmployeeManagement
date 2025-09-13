using EmployeeManagement.Models;

namespace EmployeeManagement.Interfaces
{
    public interface IEmployeeService
    {
        Task<Department> EnsureDepartmentAsync(string name);
        Task AddEmployeeAsync(Employee employee);
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<List<Employee>> GetAllEmployeesAsync();
        Task UpdateEmployeeAsync(Employee employee);   
        Task DeleteEmployeeAsync(int id);
    }


}
