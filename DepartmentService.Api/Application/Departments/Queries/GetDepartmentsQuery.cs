using DepartmentService.Api.Domain.Repositories;
using DepartmentService.Api.Dto;
using MediatR;

namespace DepartmentService.Api.Application.Departments.Queries;

public record GetDepartmentsQuery() : IRequest<IEnumerable<DepartmentReadDto>>;

public class GetDepartmentsHandler : IRequestHandler<GetDepartmentsQuery, IEnumerable<DepartmentReadDto>>
{
    private readonly IDepartmentRepository _repo;
    public GetDepartmentsHandler(IDepartmentRepository repo) => _repo = repo;

    public async Task<IEnumerable<DepartmentReadDto>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var list = await _repo.GetAllAsync(cancellationToken);
        return list.Select(d => new DepartmentReadDto { Id = d.Id, Name = d.Name });
    }
}
