using Microsoft.Extensions.DependencyInjection;

namespace HelpDesk.API.Extensions
{
    public static class CorsExtensions
    {
        private const string DefaultPolicyName = "AllowAll";

        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(DefaultPolicyName, policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            return services;
        }

        public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app)
        {
            app.UseCors(DefaultPolicyName);
            return app;
        }
    }
}
