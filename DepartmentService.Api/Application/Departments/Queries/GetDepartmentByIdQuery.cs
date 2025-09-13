using DepartmentService.Api.Domain.Repositories;
using DepartmentService.Api.Dto;
using MediatR;

namespace DepartmentService.Api.Application.Departments.Queries;

public record GetDepartmentByIdQuery(int Id) : IRequest<DepartmentReadDto?>;

public class GetDepartmentByIdHandler : IRequestHandler<GetDepartmentByIdQuery, DepartmentReadDto?>
{
    private readonly IDepartmentRepository _repo;
    public GetDepartmentByIdHandler(IDepartmentRepository repo) => _repo = repo;

    public async Task<DepartmentReadDto?> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
    {
        var d = await _repo.GetByIdAsync(request.Id, cancellationToken);
        return d is null ? null : new DepartmentReadDto { Id = d.Id, Name = d.Name };
    }
}
