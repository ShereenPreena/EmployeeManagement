using DepartmentService.Api.Domain.Repositories;
using DepartmentService.Api.Domain.ValueObjects;
using DepartmentService.Api.Dto;
using MediatR;

namespace DepartmentService.Api.Application.Departments.Queries;

public record GetDepartmentByNameQuery(string Name) : IRequest<DepartmentReadDto?>;

public class GetDepartmentByNameHandler : IRequestHandler<GetDepartmentByNameQuery, DepartmentReadDto?>
{
    private readonly IDepartmentRepository _repo;
    public GetDepartmentByNameHandler(IDepartmentRepository repo) => _repo = repo;

    public async Task<DepartmentReadDto?> Handle(GetDepartmentByNameQuery request, CancellationToken cancellationToken)
    {
        var d = await _repo.GetByNameAsync(new DepartmentName(request.Name), cancellationToken);
        return d is null ? null : new DepartmentReadDto { Id = d.Id, Name = d.Name };
    }
}
