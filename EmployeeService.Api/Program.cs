using EmployeeService.Api.Application;
using EmployeeService.Api.Application.Employees.Commands;
using EmployeeService.Api.DepartmentClient;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using EmployeeService.Api.Domain.Repositories;
using EmployeeService.Api.Infrastructure.Repositories;
using EmployeeService.Api.Persistence;
using Polly;
using Polly.Extensions.Http;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/employee-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

// EF Core
var cs = builder.Configuration.GetConnectionString("Default")!;
builder.Services.AddDbContext<EmployeeDbContext>(opt => opt.UseSqlServer(cs));
builder.Services.AddScoped<IEmployeeRepository, EfEmployeeRepository>();

// MediatR & FluentValidation
builder.Services.AddMediatR(typeof(CreateEmployeeCommand));
builder.Services.AddValidatorsFromAssemblyContaining<CreateEmployeeValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Resilient HttpClient to DepartmentService
var deptBase = builder.Configuration["Services:DepartmentService"]!;
builder.Services.AddHttpClient<IDepartmentClient, DepartmentClient>(client =>
{
    client.BaseAddress = new Uri(deptBase);
})
    .AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(new[] { TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(1) }));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler(new ExceptionHandlerOptions
{
    AllowStatusCode404Response = true,
    ExceptionHandler = async context =>
    {
        var feat = context.Features.Get<IExceptionHandlerPathFeature>();
        var problem = Results.Problem(
            title: "Unhandled exception",
            detail: feat?.Error.Message,
            statusCode: 500);
        await problem.ExecuteAsync(context);
    }
});

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
