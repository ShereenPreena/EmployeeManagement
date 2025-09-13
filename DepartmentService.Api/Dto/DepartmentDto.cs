namespace DepartmentService.Api.Dto
{
    public class DepartmentCreateDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public sealed class DepartmentUpdateDto : DepartmentCreateDto { }

    public sealed class DepartmentReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}