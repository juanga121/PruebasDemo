using Microsoft.EntityFrameworkCore;
using PruebasDemo.Configuration;
using PruebasDemo.Constants;
using PruebasDemo.Middlewares;
using Serilog;
using Serilog.Events;
using FluentValidation;
using PruebasDemo.Application.Credits.Commands.CreateCredit;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override(ApiConstants.LogSourceMicrosoft, LogEventLevel.Error)
    .MinimumLevel.Override(ApiConstants.LogSourceSystem, LogEventLevel.Error)
    .MinimumLevel.Override(ApiConstants.LogSourceEFCore, LogEventLevel.Error)

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

builder.Services.AddValidatorsFromAssemblyContaining<CreateCreditCommandValidator>();

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

if (!app.Environment.IsEnvironment(ApiConstants.TestingEnv))
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<PruebasDemo.Infrastructure.Data.DataContext>();
        await db.Database.MigrateAsync();
    }
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(ApiConstants.AllowAll);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

public partial class Program { private Program() { } }