using HelpDesk.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HelpDesk.API.Middleware
{
    /// <summary>
    /// Middleware para auditoría de seguridad HTTP.
    /// Registra todos los requests para trazabilidad y detección de patrones sospechosos.
    /// Complementa al AuditService que solo audita operaciones de negocio.
    /// </summary>
    public class SecurityAuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityAuditMiddleware> _logger;

        public SecurityAuditMiddleware(RequestDelegate next, ILogger<SecurityAuditMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ISecurityAuditService securityAudit)
        {
            // Iniciar cronómetro
            var stopwatch = Stopwatch.StartNew();

            // Capturar información del request
            var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var method = context.Request.Method;
            var path = context.Request.Path.Value ?? "/";
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var userAgent = context.Request.Headers["User-Agent"].ToString();

            int statusCode = 200;
            string? errorMessage = null;

            try
            {
                // Ejecutar el siguiente middleware
                await _next(context);

                // Capturar status code
                statusCode = context.Response.StatusCode;
            }
            catch (Exception ex)
            {
                // Capturar errores
                statusCode = 500;
                errorMessage = ex.Message;
                _logger.LogError(ex, "Error en request {Method} {Path}", method, path);
                throw; // Re-lanzar para que el GlobalExceptionHandler lo maneje
            }
            finally
            {
                // Detener cronómetro
                stopwatch.Stop();

                // Solo auditar requests relevantes para seguridad
                if (ShouldAudit(method, path, statusCode))
                {
                    try
                    {
                        await securityAudit.LogRequestAsync(
                            userId,
                            method,
                            path,
                            statusCode,
                            ipAddress,
                            userAgent,
                            stopwatch.ElapsedMilliseconds,
                            errorMessage);
                    }
                    catch (Exception ex)
                    {
                        // No fallar el request si falla el audit log
                        _logger.LogError(ex, "Error al guardar audit log de seguridad");
                    }
                }
            }
        }

        /// <summary>
        /// Determina si un request debe ser auditado.
        /// </summary>
        private static bool ShouldAudit(string method, string path, int statusCode)
        {
            // No auditar requests de recursos estáticos
            if (path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/_framework", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/css", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/js", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/favicon", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // Auditar siempre:
            // - Autenticación y autorización
            if (path.StartsWith("/api/auth", StringComparison.OrdinalIgnoreCase))
                return true;

            // - Operaciones de escritura (POST, PUT, DELETE)
            if (method != "GET" && method != "HEAD" && method != "OPTIONS")
                return true;

            // - Errores de autenticación/autorización
            if (statusCode == 401 || statusCode == 403)
                return true;

            // - Errores del servidor
            if (statusCode >= 500)
                return true;

            // No auditar GETs exitosos
            return false;
        }
    }
}
