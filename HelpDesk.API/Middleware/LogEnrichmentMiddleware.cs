using Serilog.Context;
using System.Security.Claims;

namespace HelpDesk.API.Middleware
{
    public class LogEnrichmentMiddleware
    {
        private readonly RequestDelegate _next;

        public LogEnrichmentMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Enriquecer logs con información del usuario si está autenticado
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userEmail = context.User.FindFirst(ClaimTypes.Email)?.Value;
                var userName = context.User.FindFirst(ClaimTypes.Name)?.Value;

                using (LogContext.PushProperty("UserId", userId))
                using (LogContext.PushProperty("UserEmail", userEmail))
                using (LogContext.PushProperty("UserName", userName))
                {
                    await _next(context);
                }
            }
            else
            {
                await _next(context);
            }
        }
    }

    public static class LogEnrichmentMiddlewareExtensions
    {
        public static IApplicationBuilder UseLogEnrichment(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LogEnrichmentMiddleware>();
        }
    }
}
