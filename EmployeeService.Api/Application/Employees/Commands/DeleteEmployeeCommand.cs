using EmployeeService.Api.Domain.Repositories;
using MediatR;

namespace EmployeeService.Api.Application.Employees.Commands;

public record DeleteEmployeeCommand(int Id) : IRequest<bool>;

public class DeleteEmployeeHandler : IRequestHandler<DeleteEmployeeCommand, bool>
{
    private readonly IEmployeeRepository _repo;
    public DeleteEmployeeHandler(IEmployeeRepository repo) => _repo = repo;

    public async Task<bool> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var e = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (e is null) return false;
        _repo.Remove(e);
        await _repo.SaveChangesAsync(cancellationToken);
        return true;
    }
}
