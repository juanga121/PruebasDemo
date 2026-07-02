using PruebasDemo.Configuration;
using PruebasDemo.Constants;
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

    .WriteTo.Console(outputTemplate: ApiConstants.OutputTemplate)

    .WriteTo.File(
        ApiConstants.LogPath,
        rollingInterval: RollingInterval.Day,
        outputTemplate: ApiConstants.OutputTemplate
    )
    .CreateLogger();

// Add services to the container
builder.Services.AddDatabase(builder.Configuration, builder.Environment);

builder.Services.AddCors(options =>
{
    options.AddPolicy(ApiConstants.AllowAll, builder =>
    {
        builder.WithOrigins(ApiConstants.CorsOriginLocal).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});

builder.Services.AddControllers();

builder.Services.AddFluentValidationClientsideAdapters();

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

app.UseCors(ApiConstants.AllowAll);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

public partial class Program { }