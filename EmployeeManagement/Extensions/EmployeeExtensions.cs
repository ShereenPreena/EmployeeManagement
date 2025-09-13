
using System.Globalization;
using EmployeeManagement.Models;

namespace EmployeeManagement.Extensions
{
    public static class EmployeeExtensions
    {
      
        public static string ToTitleCaseName(this string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return string.Empty;

            var normalized = string.Join(" ",
                name.Trim()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.ToLowerInvariant()));

            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            return ti.ToTitleCase(normalized);
        }

        public static IEnumerable<Employee> InDepartment(this IEnumerable<Employee> employees, string? department)
        {
            if (employees is null) return Enumerable.Empty<Employee>();
            if (string.IsNullOrWhiteSpace(department)) return employees;

            return employees.Where(e =>
                e.Department != null &&
                string.Equals(e.Department.Name, department, StringComparison.OrdinalIgnoreCase));
        }

        public static double AverageAge(this IEnumerable<Employee> employees)
        {
            if (employees is null) return 0d;
            var list = employees as ICollection<Employee> ?? employees.ToList();
            return list.Count == 0 ? 0d : list.Average(e => (double)e.Age);
        }
    }
}
