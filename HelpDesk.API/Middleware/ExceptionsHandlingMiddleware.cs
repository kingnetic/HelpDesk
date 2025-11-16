using HelpDesk.Application.DTOs;
using HelpDesk.Domain.Exceptions;
using Microsoft.AspNetCore.Builder;
using System.Net;
using System.Text.Json;

namespace HelpDesk.Api.Middleware
{
    public class ExceptionsHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionsHandlingMiddleware> _logger;

        public ExceptionsHandlingMiddleware(RequestDelegate next, ILogger<ExceptionsHandlingMiddleware> logger)
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
            catch (DomainException ex)
            {
                _logger.LogWarning(ex, $"Domain exception ocurred: {ex.Message}");
                await HandleExceptionAsync(context, ex.Message, "DOMAIN_ERROR", HttpStatusCode.BadRequest);
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning(ex, $"Application exception ocurred: {ex.Message}");
                await HandleExceptionAsync(context, ex.Message, "APP_ERROR", HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"unexpected exception ocurred: {ex.Message}");
                await HandleExceptionAsync(context, "Server exception ocurred", "SERVER_ERROR", HttpStatusCode.InternalServerError);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, string message, string errorCode, HttpStatusCode statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = BaseResponse<string>.Failure(message, errorCode);

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }

    public static class ExceptionsHandlingMiddlewareExtension
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionsHandlingMiddleware>();
        }
    }
}
