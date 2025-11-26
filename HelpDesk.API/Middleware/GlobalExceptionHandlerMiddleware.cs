using System.Net;
using System.Text.Json;
using HelpDesk.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HelpDesk.API.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocurrió una excepción no controlada: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            ValidationException validationEx => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = validationEx.Message,
                Errors = validationEx.Errors,
                Type = "ValidationError"
            },
            ConflictException conflictEx => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.Conflict,
                Message = conflictEx.Message,
                Type = "ConflictError"
            },
            NotFoundException notFoundEx => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = notFoundEx.Message,
                Type = "NotFoundError"
            },
            UnauthorizedException unauthorizedEx => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
                Message = unauthorizedEx.Message,
                Type = "UnauthorizedError"
            },
            ForbiddenException forbiddenEx => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.Forbidden,
                Message = forbiddenEx.Message,
                Type = "ForbiddenError"
            },
            DomainException domainEx => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = domainEx.Message,
                Type = "DomainError"
            },
            _ => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "Ocurrió un error interno del servidor. Por favor intente más tarde.",
                Type = "InternalServerError",
                Detail = exception.Message // Solo en desarrollo
            }
        };

        context.Response.StatusCode = response.StatusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Detail { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
