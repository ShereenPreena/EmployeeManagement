namespace EmployeeService.Api.DepartmentClient
{
    public interface IDepartmentClient
    {
        Task<bool> DepartmentExistsAsync(int departmentId, CancellationToken ct = default);
    }
}
