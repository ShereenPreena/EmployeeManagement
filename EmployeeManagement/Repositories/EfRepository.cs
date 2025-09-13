
using System.Linq.Expressions;
using EmployeeManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _db;
        protected readonly DbSet<T> _set;

        public EfRepository(AppDbContext db)
        {
            _db = db;
            _set = db.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(object id, CancellationToken ct = default)
            => await _set.FindAsync(new[] { id }, ct);

        public virtual async Task<List<T>> GetAllAsync(CancellationToken ct = default)
            => await _set.AsNoTracking().ToListAsync(ct);

        public virtual async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => await _set.AsNoTracking().Where(predicate).ToListAsync(ct);

        public virtual async Task<T> AddAsync(T entity, CancellationToken ct = default)
        {
            _set.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
        {
            _set.AddRange(entities);
            await _db.SaveChangesAsync(ct);
        }

        public virtual async Task UpdateAsync(T entity, CancellationToken ct = default)
        {
            _set.Update(entity);
            await _db.SaveChangesAsync(ct);
        }

        public virtual async Task RemoveAsync(T entity, CancellationToken ct = default)
        {
            _set.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }

        public virtual async Task RemoveRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
        {
            _set.RemoveRange(entities);
            await _db.SaveChangesAsync(ct);
        }
    }
}
