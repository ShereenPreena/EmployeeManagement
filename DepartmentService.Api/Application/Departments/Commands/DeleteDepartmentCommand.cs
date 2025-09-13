using DepartmentService.Api.Domain.Repositories;
using MediatR;

namespace DepartmentService.Api.Application.Departments.Commands;

public record DeleteDepartmentCommand(int Id) : IRequest<bool>;

public class DeleteDepartmentHandler : IRequestHandler<DeleteDepartmentCommand, bool>
{
    private readonly IDepartmentRepository _repo;
    public DeleteDepartmentHandler(IDepartmentRepository repo) => _repo = repo;

    public async Task<bool> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var d = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (d is null) return false;
        _repo.Remove(d);
        await _repo.SaveChangesAsync(cancellationToken);
        return true;
    }
}
