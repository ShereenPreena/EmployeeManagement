namespace EmployeeService.Api.Domain.ValueObjects
{
    public readonly record struct EmployeeName
    {
        public string Value { get; }
        public EmployeeName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name cannot be empty", nameof(value));
            Value = value;
        }
        public override string ToString() => Value;
        public static implicit operator string(EmployeeName name) => name.Value;
        public static implicit operator EmployeeName(string value) => new EmployeeName(value);
    }
}