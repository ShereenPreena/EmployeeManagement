using DepartmentService.Api.Domain.Entities;
using DepartmentService.Api.Domain.Repositories;
using DepartmentService.Api.Domain.ValueObjects;
using DepartmentService.Api.Dto;

namespace DepartmentService.Api.Application
{
    public class DepartmentAppService
    {
        private readonly IDepartmentRepository _repo;
        public DepartmentAppService(IDepartmentRepository repo) => _repo = repo;

        public async Task<IEnumerable<DepartmentReadDto>> GetAllAsync(CancellationToken ct)
        {
            var list = await _repo.GetAllAsync(ct);
            return list.Select(d => new DepartmentReadDto { Id = d.Id, Name = d.Name });
        }

        public async Task<DepartmentReadDto?> GetByIdAsync(int id, CancellationToken ct)
        {
            var d = await _repo.GetByIdAsync(id, ct);
            return d is null ? null : new DepartmentReadDto { Id = d.Id, Name = d.Name };
        }

        public async Task<DepartmentReadDto?> GetByNameAsync(string name, CancellationToken ct)
        {
            var d = await _repo.GetByNameAsync(new DepartmentName(name), ct);
            return d is null ? null : new DepartmentReadDto { Id = d.Id, Name = d.Name };
        }

        public async Task<DepartmentReadDto?> CreateAsync(DepartmentCreateDto dto, CancellationToken ct)
        {
            var name = new DepartmentName(dto.Name);
            if (await _repo.ExistsByNameAsync(name, ct)) return null;
            var dept = new Department(name);
            _repo.Add(dept);
            await _repo.SaveChangesAsync(ct);
            return new DepartmentReadDto { Id = dept.Id, Name = dept.Name };
        }

        public async Task<bool> UpdateAsync(int id, DepartmentUpdateDto dto, CancellationToken ct)
        {
            var d = await _repo.GetByIdAsync(id, ct);
            if (d is null) return false;
            d.Update(new DepartmentName(dto.Name));
            await _repo.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var d = await _repo.GetByIdAsync(id, ct);
            if (d is null) return false;
            _repo.Remove(d);
            await _repo.SaveChangesAsync(ct);
            return true;
        }
    }
}