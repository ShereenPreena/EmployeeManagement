using DepartmentService.Api.Domain.Entities;
using DepartmentService.Api.Domain.Repositories;
using DepartmentService.Api.Domain.ValueObjects;
using DepartmentService.Api.Dto;
using FluentValidation;
using MediatR;

namespace DepartmentService.Api.Application.Departments.Commands;

public record CreateDepartmentCommand : IRequest<DepartmentReadDto?>
{
    public string Name { get; init; } = string.Empty;
}

public class CreateDepartmentHandler : IRequestHandler<CreateDepartmentCommand, DepartmentReadDto?>
{
    private readonly IDepartmentRepository _repo;
    public CreateDepartmentHandler(IDepartmentRepository repo) => _repo = repo;

    public async Task<DepartmentReadDto?> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var name = new DepartmentName(request.Name);
        if (await _repo.ExistsByNameAsync(name, cancellationToken)) return null;
        var dept = new Department(name);
        _repo.Add(dept);
        await _repo.SaveChangesAsync(cancellationToken);
        return new DepartmentReadDto { Id = dept.Id, Name = dept.Name };
    }
}

public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
