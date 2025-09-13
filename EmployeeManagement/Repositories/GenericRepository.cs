using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Repositories
{
    public class GenericRepository<T>
    {
        private readonly List<T> _items = new();

        public void Add(T item) => _items.Add(item);
        public List<T> GetAll() => _items;
        public T? GetBy(Func<T, bool> predicate) => _items.FirstOrDefault(predicate);
    }
}
