namespace EmployeeService.Api.Dto
{
    public class EmployeeCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public decimal Salary { get; set; }
        public bool IsPermanent { get; set; }
        public int DepartmentId { get; set; }
    }

    public sealed class EmployeeUpdateDto : EmployeeCreateDto { }

    public sealed class EmployeeReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public decimal Salary { get; set; }
        public bool IsPermanent { get; set; }
        public int DepartmentId { get; set; }
    }
}
