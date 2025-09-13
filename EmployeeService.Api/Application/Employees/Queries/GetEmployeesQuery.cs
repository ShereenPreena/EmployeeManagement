using EmployeeService.Api.Domain.Repositories;
using EmployeeService.Api.Dto;
using MediatR;

namespace EmployeeService.Api.Application.Employees.Queries;

public record GetEmployeesQuery() : IRequest<IEnumerable<EmployeeReadDto>>;

public class GetEmployeesHandler : IRequestHandler<GetEmployeesQuery, IEnumerable<EmployeeReadDto>>
{
    private readonly IEmployeeRepository _repo;
    public GetEmployeesHandler(IEmployeeRepository repo) => _repo = repo;

    public async Task<IEnumerable<EmployeeReadDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        var list = await _repo.GetAllAsync(cancellationToken);
        return list.Select(e => new EmployeeReadDto
        {
            Id = e.Id,
            Name = e.Name,
            Age = e.Age,
            Salary = e.Salary,
            IsPermanent = e.IsPermanent,
            DepartmentId = e.DepartmentId
        });
    }
}
