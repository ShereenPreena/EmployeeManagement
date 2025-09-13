namespace EmployeeService.Api.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Budget { get; set; }
    }

    public class EmployeeProject
    {
        public int EmployeeId { get; set; }
        public int ProjectId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public string? RoleOnProject { get; set; }
    }
}
