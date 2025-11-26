using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace HelpDesk.API.Extensions
{
    public static class RateLimitingExtensions
    {
        public static IServiceCollection AddRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                // Global policy: 100 requests per minute per user/IP
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    var userId = context.User?.Identity?.Name;
                    var ipAddress = context.Connection?.RemoteIpAddress?.ToString();
                    var partitionKey = userId ?? ipAddress ?? "anonymous";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: partitionKey,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 100,
                            Window = TimeSpan.FromMinutes(1)
                        });
                });

                // Strict policy for authentication endpoints: 5 requests per minute per IP
                options.AddPolicy("auth", context =>
                {
                    var ipAddress = context.Connection?.RemoteIpAddress?.ToString() ?? "anonymous";
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: ipAddress,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1)
                        });
                });

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            return services;
        }
    }
}
