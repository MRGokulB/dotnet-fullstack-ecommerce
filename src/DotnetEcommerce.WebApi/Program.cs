using System.Diagnostics;
using DotnetEcommerce.Application;
using DotnetEcommerce.Infrastructure;
using DotnetEcommerce.Infrastructure.Persistence;
using DotnetEcommerce.WebApi.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "DotnetEcommerce API",
        Version = "v1",
        Description = "Enterprise E-Commerce REST API powered by .NET 7/8, EF Core, and Stripe"
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbInitializer.SeedAsync(dbContext);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapGet("/health", async (ApplicationDbContext dbContext) =>
{
    var canConnect = false;
    try
    {
        canConnect = await dbContext.Database.CanConnectAsync();
    }
    catch
    {
        canConnect = false;
    }

    var process = Process.GetCurrentProcess();
    var uptime = DateTime.UtcNow - process.StartTime.ToUniversalTime();

    return Results.Ok(new
    {
        status = canConnect ? "Healthy" : "Degraded",
        databaseConnected = canConnect,
        uptime = $"{uptime.Hours:D2}h:{uptime.Minutes:D2}m:{uptime.Seconds:D2}s",
        memoryMb = Math.Round((double)GC.GetTotalMemory(false) / (1024 * 1024), 2),
        runtime = Environment.Version.ToString(),
        timestamp = DateTime.UtcNow
    });
});

app.MapControllers();

app.Run();
