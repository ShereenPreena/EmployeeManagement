using EmployeeManagement.Data;
using EmployeeManagement.Models;
using EmployeeManagement.Repositories;
using EmployeeManagement.Services;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Tests
{
    public class EmployeeServiceTests
    {
        private static AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;

            return new AppDbContext(options);
        }

        private static EmployeeService CreateService(AppDbContext db)
        {
            var repo = new EmployeeRepository(db);
            return new EmployeeService(db, repo);
        }

        [Fact]
        public async Task AddEmployeeAsync_Should_Add_Employee()
        {
            // Arrange
            await using var db = CreateInMemoryContext();
            var service = CreateService(db);

            var dept = await service.EnsureDepartmentAsync("HR");

            var employee = new Employee
            {
                Name = "Alice",
                Age = 28,
                Salary = 60000,
                IsPermanent = true,
                DepartmentId = dept.Id
            };

            // Act
            await service.AddEmployeeAsync(employee);
            var saved = await service.GetEmployeeByIdAsync(employee.Id);

            // Assert
            Assert.NotNull(saved);
            Assert.Equal("Alice", saved!.Name);
            Assert.Equal("HR", saved.Department!.Name);
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_Should_Return_Null_If_Not_Found()
        {
            // Arrange
            await using var db = CreateInMemoryContext();
            var service = CreateService(db);

            // Act
            var result = await service.GetEmployeeByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllEmployeesAsync_Should_Return_All_Added_Employees()
        {
            // Arrange
            await using var db = CreateInMemoryContext();
            var service = CreateService(db);

            var it = await service.EnsureDepartmentAsync("IT");
            var fin = await service.EnsureDepartmentAsync("Finance");

            await service.AddEmployeeAsync(new Employee
            {
                Name = "John",
                Age = 25,
                Salary = 50000,
                IsPermanent = true,
                DepartmentId = it.Id
            });

            await service.AddEmployeeAsync(new Employee
            {
                Name = "Jane",
                Age = 30,
                Salary = 70000,
                IsPermanent = true,
                DepartmentId = fin.Id
            });

            // Act
            var all = await service.GetAllEmployeesAsync();

            // Assert
            Assert.Equal(2, all.Count);
            Assert.Contains(all, e => e.Name == "John" && e.Department!.Name == "IT");
            Assert.Contains(all, e => e.Name == "Jane" && e.Department!.Name == "Finance");
        }
    }
}
