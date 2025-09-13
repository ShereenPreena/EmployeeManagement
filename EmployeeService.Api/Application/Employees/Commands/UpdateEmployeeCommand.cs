using EmployeeService.Api.DepartmentClient;
using EmployeeService.Api.Domain.Repositories;
using EmployeeService.Api.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace EmployeeService.Api.Application.Employees.Commands;

public record UpdateEmployeeCommand : IRequest<bool>
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Age { get; init; }
    public decimal Salary { get; init; }
    public bool IsPermanent { get; init; }
    public int DepartmentId { get; init; }
}

public class UpdateEmployeeHandler : IRequestHandler<UpdateEmployeeCommand, bool>
{
    private readonly IEmployeeRepository _repo;
    private readonly IDepartmentClient _dept;
    public UpdateEmployeeHandler(IEmployeeRepository repo, IDepartmentClient dept)
    { _repo = repo; _dept = dept; }

    public async Task<bool> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var e = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (e is null) return false;
        if (!await _dept.DepartmentExistsAsync(request.DepartmentId, cancellationToken)) return false;
        e.Update(new EmployeeName(request.Name), request.Age, request.Salary, request.IsPermanent, request.DepartmentId);
        await _repo.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Age).GreaterThan(0);
        RuleFor(x => x.Salary).GreaterThan(0);
        RuleFor(x => x.DepartmentId).GreaterThan(0);
    }
}
