using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HelpDesk.Infrastructure.Observability
{
    public static class ObservabilityExtensions
    {
        public static IServiceCollection AddInfrastructureObservability(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddSerilogElastic(config);
            return services;
        }
    }
}
