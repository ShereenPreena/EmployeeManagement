using DepartmentService.Api.Domain.Repositories;
using DepartmentService.Api.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace DepartmentService.Api.Application.Departments.Commands;

public record UpdateDepartmentCommand : IRequest<bool>
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

public class UpdateDepartmentHandler : IRequestHandler<UpdateDepartmentCommand, bool>
{
    private readonly IDepartmentRepository _repo;
    public UpdateDepartmentHandler(IDepartmentRepository repo) => _repo = repo;

    public async Task<bool> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var d = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (d is null) return false;
        d.Update(new DepartmentName(request.Name));
        await _repo.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentCommand>
{
    public UpdateDepartmentValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty();
    }
}
