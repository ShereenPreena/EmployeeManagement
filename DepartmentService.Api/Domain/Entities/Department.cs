using DepartmentService.Api.Domain.ValueObjects;

namespace DepartmentService.Api.Domain.Entities
{
    // Aggregate Root representing a department
    public class Department
    {
        public int Id { get; private set; }
        public DepartmentName Name { get; private set; }

        private Department() { }

        public Department(DepartmentName name)
        {
            Update(name);
        }

        public void Update(DepartmentName name)
        {
            Name = name;
        }
    }
}