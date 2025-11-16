using HelpDesk.Application.Mappings;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HelpDesk.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // MediatR
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // Mapster
            MapsterConfig.RegisterMappings();

            return services;
        }
    }
}
