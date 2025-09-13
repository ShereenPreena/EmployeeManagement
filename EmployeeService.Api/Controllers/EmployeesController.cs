using EmployeeService.Api.Application.Employees.Commands;
using EmployeeService.Api.Application.Employees.Queries;
using EmployeeService.Api.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public EmployeesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeReadDto>>> GetAll(CancellationToken ct)
            => Ok(await _mediator.Send(new GetEmployeesQuery(), ct));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeReadDto>> GetById(int id, CancellationToken ct)
        {
            var e = await _mediator.Send(new GetEmployeeByIdQuery(id), ct);
            return e is null ? NotFound() : Ok(e);
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeReadDto>> Create([FromBody] CreateEmployeeCommand command, CancellationToken ct)
        {
            var created = await _mediator.Send(command, ct);
            return created is null
                ? BadRequest($"Department {command.DepartmentId} does not exist.")
                : CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeCommand command, CancellationToken ct)
            => await _mediator.Send(command with { Id = id }, ct) ? NoContent() : NotFound();

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
            => await _mediator.Send(new DeleteEmployeeCommand(id), ct) ? NoContent() : NotFound();
    }
}
