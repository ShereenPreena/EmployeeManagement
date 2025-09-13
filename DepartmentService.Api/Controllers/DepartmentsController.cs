using DepartmentService.Api.Application.Departments.Commands;
using DepartmentService.Api.Application.Departments.Queries;
using DepartmentService.Api.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DepartmentService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DepartmentsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentReadDto>>> GetAll(CancellationToken ct)
            => Ok(await _mediator.Send(new GetDepartmentsQuery(), ct));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DepartmentReadDto>> GetById(int id, CancellationToken ct)
        {
            var d = await _mediator.Send(new GetDepartmentByIdQuery(id), ct);
            return d is null ? NotFound() : Ok(d);
        }

        [HttpGet("by-name/{name}")]
        public async Task<ActionResult<DepartmentReadDto>> GetByName(string name, CancellationToken ct)
        {
            var d = await _mediator.Send(new GetDepartmentByNameQuery(name), ct);
            return d is null ? NotFound() : Ok(d);
        }

        [HttpPost]
        public async Task<ActionResult<DepartmentReadDto>> Create([FromBody] CreateDepartmentCommand command, CancellationToken ct)
        {
            var created = await _mediator.Send(command, ct);
            return created is null
                ? Conflict("Department name already exists.")
                : CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDepartmentCommand command, CancellationToken ct)
            => await _mediator.Send(command with { Id = id }, ct) ? NoContent() : NotFound();

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
            => await _mediator.Send(new DeleteDepartmentCommand(id), ct) ? NoContent() : NotFound();
    }
}
