using EmployeeService.Api.Data;
using EmployeeService.Api.DepartmentClient;
using EmployeeService.Api.Dto;
using EmployeeService.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeDbContext _db;
        private readonly IDepartmentClient _dept;

        public EmployeesController(EmployeeDbContext db, IDepartmentClient dept)
        {
            _db = db; _dept = dept;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeReadDto>>> GetAll()
        {
            var list = await _db.Employees.AsNoTracking().ToListAsync();
            return Ok(list.Select(e => new EmployeeReadDto
            {
                Id = e.Id,
                Name = e.Name,
                Age = e.Age,
                Salary = e.Salary,
                IsPermanent = e.IsPermanent,
                DepartmentId = e.DepartmentId
            }));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeReadDto>> GetById(int id)
        {
            var e = await _db.Employees.FindAsync(id);
            if (e is null) return NotFound();
            return Ok(new EmployeeReadDto
            {
                Id = e.Id,
                Name = e.Name,
                Age = e.Age,
                Salary = e.Salary,
                IsPermanent = e.IsPermanent,
                DepartmentId = e.DepartmentId
            });
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeReadDto>> Create([FromBody] EmployeeCreateDto dto, CancellationToken ct)
        {
            if (!await _dept.DepartmentExistsAsync(dto.DepartmentId, ct))
                return BadRequest($"Department {dto.DepartmentId} does not exist.");

            var e = new Employee
            {
                Name = dto.Name,
                Age = dto.Age,
                Salary = dto.Salary,
                IsPermanent = dto.IsPermanent,
                DepartmentId = dto.DepartmentId
            };
            _db.Employees.Add(e);
            await _db.SaveChangesAsync(ct);

            var read = new EmployeeReadDto
            {
                Id = e.Id,
                Name = e.Name,
                Age = e.Age,
                Salary = e.Salary,
                IsPermanent = e.IsPermanent,
                DepartmentId = e.DepartmentId
            };
            return CreatedAtAction(nameof(GetById), new { id = e.Id }, read);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeUpdateDto dto, CancellationToken ct)
        {
            var e = await _db.Employees.FindAsync([id], ct);
            if (e is null) return NotFound();

            if (!await _dept.DepartmentExistsAsync(dto.DepartmentId, ct))
                return BadRequest($"Department {dto.DepartmentId} does not exist.");

            e.Name = dto.Name; e.Age = dto.Age; e.Salary = dto.Salary;
            e.IsPermanent = dto.IsPermanent; e.DepartmentId = dto.DepartmentId;

            await _db.SaveChangesAsync(ct);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var e = await _db.Employees.FindAsync([id], ct);
            if (e is null) return NotFound();
            _db.Remove(e);
            await _db.SaveChangesAsync(ct);
            return NoContent();
        }
    }
}
