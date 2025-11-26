using HelpDesk.API.Extensions;
using HelpDesk.Application;
using HelpDesk.Infrastructure;
using HelpDesk.Infrastructure.Persistence;
using Serilog;
using HelpDesk.API.Middleware;
using System.Threading.RateLimiting;

// Configuración de Serilog para logging estructurado
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .WriteTo.File("logs/helpdesk-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.WithProperty("Application", "HelpDesk")
    .MinimumLevel.Information()
    .CreateLogger();

try
{
    Log.Information("Iniciando HelpDesk API");

    var builder = WebApplication.CreateBuilder(args);

    // Agregar Aspire - ServiceDefaults (OpenTelemetry, Health Checks, Métricas)
    builder.AddServiceDefaults();

    // Health Checks específicos de la aplicación (BD, SMTP)
    builder.AddApplicationHealthChecks();

    // Agregar Serilog
    builder.Host.UseSerilog();

    // CORS
    //builder.Services.AddCorsPolicy();

    builder.Services.AddHttpContextAccessor();

    //builder.Services.AddRateLimiting();

    // Capa de aplicación
    builder.Services.AddApplication();

    // Servicios de API (FluentValidation, etc.)
    builder.Services.AddApiServices();

    // Capa de infraestructura
    builder.Services.AddInfrastructure(builder.Configuration);

    // Controladores
    builder.Services.AddControllers();

    // Swagger
    builder.Services.AddSwaggerDocumentation();

    var app = builder.Build();

    // Agregar logging de requests con Serilog
    app.UseSerilogRequestLogging();

    // Agregar manejador global de excepciones (debe estar temprano en el pipeline)
    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

    //app.UseCorsPolicy();

    // Security Headers - CSP, XSS Protection, etc. (temprano en el pipeline)
    //app.UseMiddleware<SecurityHeadersMiddleware>();

    // Agregar logs Serilog detallados
    app.UseLogEnrichment();



    // Swagger
    app.UseSwaggerDocumentation();

    // Habilitar archivos estáticos (HTML, CSS, JS)
    app.UseStaticFiles();

    app.UseHttpsRedirection(); // Disabled to allow HTTP authentication

    app.UseAuthentication();
    app.UseAuthorization();

    // Rate Limiting
    app.UseRateLimiter();

    // Auditoría de seguridad HTTP (después de autenticación para capturar userId)
    app.UseMiddleware<HelpDesk.API.Middleware.SecurityAuditMiddleware>();

    app.MapControllers();

    // Mapear endpoints de Aspire: health checks y métricas
    app.MapDefaultEndpoints();


    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        await DbSeeder.SeedAsync(scope.ServiceProvider);
    }


    // Inicializar datos de prueba
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            await DbSeeder.SeedAsync(scope.ServiceProvider);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Ocurrió un error al inicializar la base de datos");
        }
    }

    Log.Information("HelpDesk API iniciado correctamente");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación terminó inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}
