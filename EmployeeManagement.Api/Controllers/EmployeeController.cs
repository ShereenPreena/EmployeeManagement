using EmployeeManagement.Api.Dto;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Models;
using EmployeeManagement.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeeController(IEmployeeService service)
        {
            _service = service;
        }

        // GET /api/employee
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmployeeReadDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeeReadDto>>> GetAll()
        {
            var employees = await _service.GetAllEmployeesAsync();

            var dto = employees.Select(e => new EmployeeReadDto
            {
                Id = e.Id,
                Name = e.Name,
                Age = e.Age,
                Salary = e.Salary,
                IsPermanent = e.IsPermanent,
                Department = e.Department?.Name
            });

            return Ok(dto); // 200
        }

        // GET /api/employee/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EmployeeReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeReadDto>> GetById(int id)
        {
            var e = await _service.GetEmployeeByIdAsync(id);
            if (e is null) return NotFound(); 

            return Ok(new EmployeeReadDto
            {
                Id = e.Id,
                Name = e.Name,
                Age = e.Age,
                Salary = e.Salary,
                IsPermanent = e.IsPermanent,
                Department = e.Department?.Name
            });
        }

        // POST /api/employee
        [HttpPost]
        [ProducesResponseType(typeof(EmployeeReadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmployeeReadDto>> Create([FromBody] EmployeeCreateDto dto)
        { 
            var dept = await _service.EnsureDepartmentAsync(dto.DepartmentName);

            var emp = new Employee
            {
                Name = dto.Name,
                Age = dto.Age,
                Salary = dto.Salary,
                IsPermanent = dto.IsPermanent,
                DepartmentId = dept.Id
            };

            await _service.AddEmployeeAsync(emp);

            var read = new EmployeeReadDto
            {
                Id = emp.Id,
                Name = emp.Name,
                Age = emp.Age,
                Salary = emp.Salary,
                IsPermanent = emp.IsPermanent,
                Department = dept.Name
            };

            return CreatedAtAction(nameof(GetById), new { id = emp.Id }, read);
        }

        // PUT /api/employee/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeUpdateDto dto)
        {
            var existing = await _service.GetEmployeeByIdAsync(id);
            if (existing is null) return NotFound();

            var dept = await _service.EnsureDepartmentAsync(dto.DepartmentName);

            existing.Name = dto.Name;
            existing.Age = dto.Age;
            existing.Salary = dto.Salary;
            existing.IsPermanent = dto.IsPermanent;
            existing.DepartmentId = dept.Id;

            await _service.UpdateEmployeeAsync(existing);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _service.GetEmployeeByIdAsync(id);
            if (existing is null) return NotFound();

            await _service.DeleteEmployeeAsync(id);
            return NoContent();
        }
    }
}

