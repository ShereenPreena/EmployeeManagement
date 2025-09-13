using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.Dto
{
    public sealed class EmployeeCreateDto
    {
        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int Age { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Salary { get; set; }

        public bool IsPermanent { get; set; }

        [Required, StringLength(100)]
        public string DepartmentName { get; set; } = string.Empty;
    }

    public sealed class EmployeeUpdateDto
    {
        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int Age { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Salary { get; set; }

        public bool IsPermanent { get; set; }

        [Required, StringLength(100)]
        public string DepartmentName { get; set; } = string.Empty;
    }

    public sealed class EmployeeReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public decimal Salary { get; set; }
        public bool IsPermanent { get; set; }
        public string? Department { get; set; }
    }
}
