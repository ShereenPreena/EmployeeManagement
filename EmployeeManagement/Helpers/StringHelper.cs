
namespace EmployeeManagement.Helpers
{
    public static class StringHelper
    {
        public static string CapitalizeName(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            var parts = input.Trim().Split(' ');
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].Length > 0)
                    parts[i] = char.ToUpper(parts[i][0]) + parts[i][1..].ToLower();
            }
            return string.Join(" ", parts);
        }
    }
}
