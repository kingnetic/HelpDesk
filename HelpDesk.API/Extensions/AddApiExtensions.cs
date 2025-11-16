using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace HelpDesk.API.Extensions
{
    public static class ApiExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            // Registrar todos los Validators de HelpDesk.Application
            services.AddValidatorsFromAssembly(
                typeof(HelpDesk.Application.DependencyInjection).Assembly
            );

            return services;
        }
    }
}
