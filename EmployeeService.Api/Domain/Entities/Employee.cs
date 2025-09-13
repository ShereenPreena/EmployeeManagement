using EmployeeService.Api.Domain.ValueObjects;

namespace EmployeeService.Api.Domain.Entities
{
    // Aggregate Root representing an employee
    public class Employee
    {
        public int Id { get; private set; }
        public EmployeeName Name { get; private set; }
        public int Age { get; private set; }
        public decimal Salary { get; private set; }
        public bool IsPermanent { get; private set; }
        public int DepartmentId { get; private set; }

        private Employee() { }

        public Employee(EmployeeName name, int age, decimal salary, bool isPermanent, int departmentId)
        {
            Update(name, age, salary, isPermanent, departmentId);
        }

        public void Update(EmployeeName name, int age, decimal salary, bool isPermanent, int departmentId)
        {
            if (age < 0) throw new ArgumentOutOfRangeException(nameof(age));
            Name = name;
            Age = age;
            Salary = salary;
            IsPermanent = isPermanent;
            DepartmentId = departmentId;
        }
    }
}