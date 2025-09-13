using EmployeeManagement.Extensions;
using EmployeeManagement.Models;
using EmployeeManagement.Repositories;
using EmployeeManagement.Services;
using EmployeeManagement.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EmployeeManagement
{
    class Program
    {
        private static IHost _host = null!;

        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                _host = Host.CreateDefaultBuilder(args)
                    .UseSerilog()
                    .ConfigureServices((ctx, services) =>
                    {
                        var cs = ctx.Configuration.GetConnectionString("Default");
                        if (string.IsNullOrWhiteSpace(cs))
                        {
                            Log.Warning("Connection string 'Default' not found. Falling back to LocalDB.");
                            cs = "Server=(localdb)\\MSSQLLocalDB;Database=EmployeeManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true";
                        }

                        services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(cs));

                        // App services
                        services.AddScoped<IEmployeeService, EmployeeService>();
                        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
                        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

                        // Lifetime demo types
                        services.AddSingleton<OperationSingleton>();
                        services.AddScoped<OperationScoped>();
                        services.AddTransient<OperationTransient>();
                        services.AddTransient<OperationReporter>();
                    })
                    .Build();

                await using (var scope = _host.Services.CreateAsyncScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    await db.Database.MigrateAsync();          
                }

                await SeedSampleDataAsync(_host.Services);

                AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
                {
                    Log.Fatal((Exception)args.ExceptionObject, "Unhandled exception occurred");
                    Log.CloseAndFlush();
                };

                Log.Information("Application started.");

                bool exit = false;

                while (!exit)
                {
                    using var scope = _host.Services.CreateScope();
                    var provider = scope.ServiceProvider;

                    var employeeService = provider.GetRequiredService<IEmployeeService>();
                    var ops = provider.GetRequiredService<OperationReporter>(); 

                    ops.Report("Menu loop start");

                    Console.WriteLine("\n Menu:");
                    Console.WriteLine("1. Add Employee");
                    Console.WriteLine("2. View All Employees");
                    Console.WriteLine("3. Export as JSON");
                    Console.WriteLine("4. Filter by Department");
                    Console.WriteLine("5. Show Names & Departments");
                    Console.WriteLine("6. Show Statistics");
                    Console.WriteLine("7. Department with Employee Counts");
                    Console.WriteLine("8. Project Payrolls");
                    Console.WriteLine("9. Top Earners Per Dept");
                    Console.WriteLine("10. Employees in High-Pay Depts");
                    Console.WriteLine("11. Employees On Project");
                    Console.WriteLine("12. Exit");
                    Console.Write("Enter choice: ");

                    string? choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            await AddEmployeeFlow(employeeService);
                            break;
                        case "2":
                            await DisplayAllEmployees(employeeService);
                            break;
                        case "3":
                            await ExportEmployeesToJson(employeeService);
                            break;
                        case "4":
                            Console.Write("Enter Department: ");
                            await FilterEmployeesByDepartment(employeeService, Console.ReadLine()!);
                            break;
                        case "5":
                            await ShowEmployeeNamesAndDepartments(employeeService);
                            break;
                        case "6":
                            await ShowEmployeeStatistics(employeeService);
                            break;
                        case "7":
                            {
                                var repo = provider.GetRequiredService<IEmployeeRepository>();
                                var rows = await repo.GetEmployeeCountsByDepartmentAsync();
                                Console.WriteLine("\nDepartment | Employee Count");
                                foreach (var (dept, count) in rows)
                                    Console.WriteLine($"  {dept}: {count}");
                                break;
                            }
                        case "8":
                            {
                                var repo = provider.GetRequiredService<IEmployeeRepository>();
                                var rows = await repo.GetProjectPayrollsAsync();
                                Console.WriteLine("\nProject | Total Salary");
                                foreach (var (project, total) in rows)
                                    Console.WriteLine($"  {project}: {total}");
                                break;
                            }
                        case "9":
                            {
                                var repo = provider.GetRequiredService<IEmployeeRepository>();
                                Console.Write("Enter N for Top Earners per Department: ");
                                if (!int.TryParse(Console.ReadLine(), out var n)) n = 1;
                                var rows = await repo.GetTopEarnersPerDepartmentAsync(n);
                                Console.WriteLine("\nTop Earners Per Department");
                                foreach (var e in rows)
                                    Console.WriteLine($"  {e.Department?.Name} | {e.Name} : {e.Salary}");
                                break;
                            }
                        case "10":
                            {
                                var repo = provider.GetRequiredService<IEmployeeRepository>();
                                Console.Write("Enter Avg Salary Threshold: ");
                                if (!decimal.TryParse(Console.ReadLine(), out var threshold)) threshold = 70000m;
                                var rows = await repo.GetEmployeesInHighPayDepartmentsAsync(threshold);
                                Console.WriteLine("\nEmployees in High-Pay Departments");
                                foreach (var e in rows)
                                    Console.WriteLine($"  {e.Department?.Name} | {e.Name} : {e.Salary:F2}");
                                break;
                            }
                        case "11":
                            {
                                var repo = provider.GetRequiredService<IEmployeeRepository>();
                                Console.Write("Enter Project Id: ");
                                if (!int.TryParse(Console.ReadLine(), out var pid)) pid = 1;
                                var rows = await repo.GetEmployeesOnProjectAsync(pid);
                                Console.WriteLine($"\nEmployees on Project Id {pid}");
                                foreach (var e in rows)
                                    Console.WriteLine($"  {e.Name} ({e.Department?.Name})");
                                break;
                            }
                        case "12":
                            exit = true;
                            Console.WriteLine(" Exiting...");
                            Log.Information("Application exited by user.");
                            break;
                        default:
                            Console.WriteLine(" Invalid choice. Try again.");
                            Log.Warning("Invalid menu choice: {Choice}", choice);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled exception in main loop.");
                Console.WriteLine("An unexpected error occurred. Please check logs.");
            }
            finally
            {
                Log.Information("Application ended.");
                Log.CloseAndFlush();
                if (_host is IAsyncDisposable ad) await ad.DisposeAsync();
                else _host.Dispose();
            }
        }

        static async Task AddEmployeeFlow(IEmployeeService employeeService)
        {
            try
            {
                Console.Write("\nIs this a Manager? (yes/no): ");
                bool isManager = Console.ReadLine()?.Trim().ToLower() == "yes";

                Employee emp = isManager ? new Manager() : new Employee();

                Console.Write("Enter Name: ");
               
                emp.Name = (Console.ReadLine() ?? string.Empty).ToTitleCaseName();

                Console.Write("Enter Age: ");
                int age = int.Parse(Console.ReadLine()!);
                if (age < 0) throw new EmployeeInputException("Age cannot be negative.");
                emp.Age = age;

                Console.Write("Enter Department: ");
                string deptName = Console.ReadLine()!; 

                var dept = await employeeService.EnsureDepartmentAsync(deptName);
                emp.DepartmentId = dept.Id; 

                Console.Write("Enter Salary: ");
                emp.Salary = decimal.Parse(Console.ReadLine()!);

                emp.IsPermanent = emp.Salary >= 50000m;

                if (emp is Manager manager)
                {
                    Console.Write("Enter Team Size: ");
                    manager.TeamSize = int.Parse(Console.ReadLine()!);
                }

                await employeeService.AddEmployeeAsync(emp);

                Console.WriteLine(" Employee added successfully.");
                Log.Information("Employee added: {@Employee}", emp);
            }
            catch (FormatException ex)
            {
                Log.Warning(ex, "Invalid input format.");
                Console.WriteLine(" Invalid input format. Please enter valid data types.");
            }
            catch (EmployeeInputException ex)
            {
                Log.Warning(ex, "Business rule violation.");
                Console.WriteLine($" Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error in AddEmployeeFlow.");
                Console.WriteLine(" Something went wrong. Please try again.");
            }
        }

        static async Task DisplayAllEmployees(IEmployeeService employeeService)
        {
            var employees = await employeeService.GetAllEmployeesAsync();
            Console.WriteLine("\n Employee List:");

            if (employees.Count == 0)
            {
                Console.WriteLine(" No employees found.");
                Log.Information("No employees to display.");
                return;
            }

            foreach (var emp in employees)
            {
                emp.DisplayDetails();
                Console.WriteLine("-----------------------");
            }

            Log.Information("Displayed {Count} employees.", employees.Count);
        }

        static async Task ExportEmployeesToJson(IEmployeeService employeeService)
        {
            var employees = await employeeService.GetAllEmployeesAsync();

            if (employees.Count == 0)
            {
                Console.WriteLine(" No data to export.");
                Log.Information("Export skipped. No data.");
                return;
            }

            var json = JsonSerializer.Serialize(
                employees,
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles 
                });

            Console.WriteLine("\n JSON Output:");
            Console.WriteLine(json);
            Log.Information("Exported {Count} employees as JSON.", employees.Count);
        }


        static async Task FilterEmployeesByDepartment(IEmployeeService employeeService, string department)
        {
            var employees = await employeeService.GetAllEmployeesAsync();
            var filtered = employees.InDepartment(department).ToList();

            if (!filtered.Any())
                Console.WriteLine(" No employees found in that department.");
            else
            {
                Console.WriteLine($"\n Employees in {department}:");
                foreach (var emp in filtered)
                    emp.DisplayDetails();
            }
        }

        static async Task ShowEmployeeNamesAndDepartments(IEmployeeService employeeService)
        {
            var employees = await employeeService.GetAllEmployeesAsync();
            var projection = employees.Select(e => new { e.Name, Department = e.Department?.Name ?? "-" });

            Console.WriteLine("\n Employee Names & Departments:");
            foreach (var item in projection)
                Console.WriteLine($"Name: {item.Name}, Dept: {item.Department}");
        }


        static async Task ShowEmployeeStatistics(IEmployeeService employeeService)
        {
            var employees = await employeeService.GetAllEmployeesAsync();
            if (!employees.Any())
            {
                Console.WriteLine(" No data to calculate.");
                return;
            }

            var averageAge = employees.AverageAge();
            var maxSalary = employees.Max(e => e.Salary);
            var count = employees.Count;

            Console.WriteLine($"\n Total Employees: {count}");
            Console.WriteLine($" Average Age: {averageAge}");
            Console.WriteLine($" Max Salary: {maxSalary}");
        }

        static async Task SeedSampleDataAsync(IServiceProvider provider)
        {
            using var scope = provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (await db.Employees.AnyAsync()) return;

            var depEng = new Department { Name = "Engineering" };
            var depHr = new Department { Name = "HR" };

            var pA = new Project { Name = "Apollo", Budget = 100000m };
            var pB = new Project { Name = "Borealis", Budget = 200000m };

            var e1 = new Employee { Name = "Alice", Age = 28, Salary = 80000, IsPermanent = true, Department = depEng };
            var e2 = new Employee { Name = "Bob", Age = 35, Salary = 95000, IsPermanent = true, Department = depEng };
            var e3 = new Employee { Name = "Cara", Age = 30, Salary = 45000, IsPermanent = false, Department = depHr };

            db.AddRange(depEng, depHr, pA, pB, e1, e2, e3);
            await db.SaveChangesAsync();

            db.EmployeeProjects.AddRange(
                new EmployeeProject { EmployeeId = e1.Id, ProjectId = pA.Id },
                new EmployeeProject { EmployeeId = e1.Id, ProjectId = pB.Id },
                new EmployeeProject { EmployeeId = e2.Id, ProjectId = pB.Id }
            );

            await db.SaveChangesAsync();
        }

    }


    public abstract class Operation
    {
        public Guid OperationId { get; } = Guid.NewGuid();
        public override string ToString() => $"{GetType().Name} [{OperationId}]";
    }

    public sealed class OperationSingleton : Operation { }
    public sealed class OperationScoped : Operation { }
    public sealed class OperationTransient : Operation { }
    public sealed class OperationReporter
    {
        private readonly OperationSingleton _single;
        private readonly OperationScoped _scoped;
        private readonly OperationTransient _transient;

        public OperationReporter(
            OperationSingleton single,
            OperationScoped scoped,
            OperationTransient transient)
        {
            _single = single;
            _scoped = scoped;
            _transient = transient;
        }

        public void Report(string when)
        {
            Console.WriteLine($"\n[Lifetimes @ {when}]");
            Console.WriteLine($"  Singleton : {_single}");
            Console.WriteLine($"  Scoped    : {_scoped}");
            Console.WriteLine($"  Transient : {_transient}");
        }
    }
}
