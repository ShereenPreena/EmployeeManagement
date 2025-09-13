using DepartmentService.Api.Application;
using DepartmentService.Api.Dto;
using Microsoft.AspNetCore.Mvc;

namespace DepartmentService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly DepartmentAppService _service;
        public DepartmentsController(DepartmentAppService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentReadDto>>> GetAll(CancellationToken ct)
            => Ok(await _service.GetAllAsync(ct));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DepartmentReadDto>> GetById(int id, CancellationToken ct)
        {
            var d = await _service.GetByIdAsync(id, ct);
            return d is null ? NotFound() : Ok(d);
        }

        [HttpGet("by-name/{name}")]
        public async Task<ActionResult<DepartmentReadDto>> GetByName(string name, CancellationToken ct)
        {
            var d = await _service.GetByNameAsync(name, ct);
            return d is null ? NotFound() : Ok(d);
        }

        [HttpPost]
        public async Task<ActionResult<DepartmentReadDto>> Create([FromBody] DepartmentCreateDto dto, CancellationToken ct)
        {
            var created = await _service.CreateAsync(dto, ct);
            return created is null
                ? Conflict("Department name already exists.")
                : CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }


        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] DepartmentUpdateDto dto, CancellationToken ct)
            => await _service.UpdateAsync(id, dto, ct) ? NoContent() : NotFound();

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
            => await _service.DeleteAsync(id, ct) ? NoContent() : NotFound();
    }
}
