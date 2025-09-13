namespace DepartmentService.Api.Domain.ValueObjects
{
    public readonly record struct DepartmentName
    {
        public string Value { get; }
        public DepartmentName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name cannot be empty", nameof(value));
            Value = value;
        }
        public override string ToString() => Value;
        public static implicit operator string(DepartmentName name) => name.Value;
        public static implicit operator DepartmentName(string value) => new DepartmentName(value);
    }
}