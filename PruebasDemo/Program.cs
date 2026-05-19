using PruebasDemo.Configuration;
using PruebasDemo.Middlewares;
using Serilog;
using Serilog.Events;
using FluentValidation;
using PruebasDemo.Application.Validators;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
    .MinimumLevel.Override("System", LogEventLevel.Error)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error)
    .WriteTo.Console(outputTemplate:
        "{Timestamp:yyyy-MM-dd HH:mm:ss} - {Message:lj}{NewLine}")
    .WriteTo.File(
        "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} - {Message:lj}{NewLine}"
    )
    .CreateLogger();

// Add services to the container
builder.Services.AddDatabase(builder.Configuration, builder.Environment);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<CreditoDtoValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRepositoryDependency();

builder.Host.UseSerilog();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

public partial class Program { }