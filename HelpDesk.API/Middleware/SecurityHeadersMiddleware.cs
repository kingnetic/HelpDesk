using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace HelpDesk.API.Middleware
{
    /// <summary>
    /// Middleware para agregar headers de seguridad (CSP, XSS Protection, etc.)
    /// Previene ataques XSS, clickjacking y otros vectores de ataque comunes.
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // X-Content-Type-Options: Previene MIME type sniffing
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

            // X-Frame-Options: Previene clickjacking
            context.Response.Headers.Append("X-Frame-Options", "DENY");

            // X-XSS-Protection: Habilita protección XSS del navegador
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

            // Referrer-Policy: Controla información de referrer
            context.Response.Headers.Append("Referrer-Policy", "no-referrer");

            // Content-Security-Policy: Previene XSS y otros ataques de inyección
            context.Response.Headers.Append("Content-Security-Policy",
                "default-src 'self'; " +
                "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " + // Permite scripts inline (necesario para Swagger)
                "style-src 'self' 'unsafe-inline'; " + // Permite estilos inline (necesario para Swagger)
                "img-src 'self' data: https:; " +
                "font-src 'self' data:; " +
                "connect-src 'self' http: https:; " +
                "frame-ancestors 'none'");

            // Permissions-Policy: Controla características del navegador
            context.Response.Headers.Append("Permissions-Policy",
                "geolocation=(), " +
                "microphone=(), " +
                "camera=()");

            // Strict-Transport-Security: Fuerza HTTPS (solo en producción)
            if (!context.Request.Host.Host.Contains("localhost"))
            {
                context.Response.Headers.Append("Strict-Transport-Security",
                    "max-age=31536000; includeSubDomains");
            }

            await _next(context);
        }
    }
}
