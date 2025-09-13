using EmployeeService.Api.DepartmentClient;
using EmployeeService.Api.Domain.Entities;
using EmployeeService.Api.Domain.Repositories;
using EmployeeService.Api.Domain.ValueObjects;
using EmployeeService.Api.Dto;
using FluentValidation;
using MediatR;

namespace EmployeeService.Api.Application.Employees.Commands;

public record CreateEmployeeCommand : IRequest<EmployeeReadDto?>
{
    public string Name { get; init; } = string.Empty;
    public int Age { get; init; }
    public decimal Salary { get; init; }
    public bool IsPermanent { get; init; }
    public int DepartmentId { get; init; }
}

public class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, EmployeeReadDto?>
{
    private readonly IEmployeeRepository _repo;
    private readonly IDepartmentClient _dept;
    public CreateEmployeeHandler(IEmployeeRepository repo, IDepartmentClient dept)
    { _repo = repo; _dept = dept; }

    public async Task<EmployeeReadDto?> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        if (!await _dept.DepartmentExistsAsync(request.DepartmentId, cancellationToken)) return null;
        var employee = new Employee(new EmployeeName(request.Name), request.Age, request.Salary, request.IsPermanent, request.DepartmentId);
        _repo.Add(employee);
        await _repo.SaveChangesAsync(cancellationToken);
        return new EmployeeReadDto
        {
            Id = employee.Id,
            Name = employee.Name,
            Age = employee.Age,
            Salary = employee.Salary,
            IsPermanent = employee.IsPermanent,
            DepartmentId = employee.DepartmentId
        };
    }
}

public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Age).GreaterThan(0);
        RuleFor(x => x.Salary).GreaterThan(0);
        RuleFor(x => x.DepartmentId).GreaterThan(0);
    }
}
