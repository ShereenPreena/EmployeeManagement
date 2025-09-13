using DepartmentService.Api.Data;
using DepartmentService.Api.Dto;
using DepartmentService.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DepartmentService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly DepartmentDbContext _db;
        public DepartmentsController(DepartmentDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetAll()
            => Ok(await _db.Departments.AsNoTracking().ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Department>> GetById(int id)
        {
            var d = await _db.Departments.FindAsync(id);
            return d is null ? NotFound() : Ok(d);
        }

        [HttpGet("by-name/{name}")]
        public async Task<ActionResult<Department>> GetByName(string name)
        {
            var d = await _db.Departments.FirstOrDefaultAsync(x => x.Name == name);
            return d is null ? NotFound() : Ok(d);
        }

        [HttpPost]
        public async Task<ActionResult<Department>> Create([FromBody] DepartmentCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name is required.");

            var exists = await _db.Departments.AnyAsync(d => d.Name == dto.Name);
            if (exists) return Conflict("Department name already exists.");

            var entity = new Department { Name = dto.Name };
            _db.Departments.Add(entity);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }


        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Department dto)
        {
            var d = await _db.Departments.FindAsync(id);
            if (d is null) return NotFound();
            d.Name = dto.Name;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var d = await _db.Departments.FindAsync(id);
            if (d is null) return NotFound();
            _db.Remove(d);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
