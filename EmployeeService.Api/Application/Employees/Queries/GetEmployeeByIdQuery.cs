using EmployeeService.Api.Domain.Repositories;
using EmployeeService.Api.Dto;
using MediatR;

namespace EmployeeService.Api.Application.Employees.Queries;

public record GetEmployeeByIdQuery(int Id) : IRequest<EmployeeReadDto?>;

public class GetEmployeeByIdHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeReadDto?>
{
    private readonly IEmployeeRepository _repo;
    public GetEmployeeByIdHandler(IEmployeeRepository repo) => _repo = repo;

    public async Task<EmployeeReadDto?> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var e = await _repo.GetByIdAsync(request.Id, cancellationToken);
        return e is null ? null : new EmployeeReadDto
        {
            Id = e.Id,
            Name = e.Name,
            Age = e.Age,
            Salary = e.Salary,
            IsPermanent = e.IsPermanent,
            DepartmentId = e.DepartmentId
        };
    }
}
