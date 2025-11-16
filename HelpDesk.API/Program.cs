using HelpDesk.API.Extensions;
using HelpDesk.Application;
using HelpDesk.Infrastructure;
using HelpDesk.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCorsPolicy();

builder.Services.AddHttpContextAccessor();

// Application layer
builder.Services.AddApplication();

// API services (FluentValidation, etc.)
builder.Services.AddApiServices();

// Infrastructure layer
builder.Services.AddInfrastructure(builder.Configuration);

// Controllers
builder.Services.AddControllers();

// Swagger separado
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

app.UseCorsPolicy();

// Swagger
app.UseSwaggerDocumentation();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seeder
using (var scope = app.Services.CreateScope())
{
    try
    {
        await DbSeeder.SeedAsync(scope.ServiceProvider);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Seeder error: " + ex.Message);
    }
}

app.Run();
