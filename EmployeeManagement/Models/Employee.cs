namespace EmployeeManagement.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }

        public decimal Salary { get; set; }

        public bool IsPermanent { get; set; }

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        public ICollection<EmployeeProject> EmployeeProjects { get; set; } = new List<EmployeeProject>();

        public virtual void DisplayDetails()
        {
            Console.WriteLine($"Id: {Id}");
            Console.WriteLine($"Name: {Name}");
            Console.WriteLine($"Age: {Age}");
            Console.WriteLine($"Salary: {Salary}");
            Console.WriteLine($"Permanent: {IsPermanent}");
            Console.WriteLine($"DepartmentId: {DepartmentId}  ({Department?.Name ?? "-"})");
        }
    }
}
