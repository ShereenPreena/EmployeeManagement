using DepartmentService.Api.Application;
using DepartmentService.Api.Domain.Repositories;
using DepartmentService.Api.Infrastructure.Repositories;
using DepartmentService.Api.Persistence;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/department-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

var cs = builder.Configuration.GetConnectionString("Default")!;
builder.Services.AddDbContext<DepartmentDbContext>(opt => opt.UseSqlServer(cs));
builder.Services.AddScoped<IDepartmentRepository, EfDepartmentRepository>();
builder.Services.AddScoped<DepartmentAppService>();

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
        await Results.Problem(
            title: "Unhandled exception",
            detail: feat?.Error.Message,
            statusCode: 500
        ).ExecuteAsync(context);
    }
});

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
