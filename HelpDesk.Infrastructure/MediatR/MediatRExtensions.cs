using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HelpDesk.Infrastructure.MediatR
{
    public static class MediatRExtensions
    {
        public static IServiceCollection AddMediatRWithLicense(this IServiceCollection services, IConfiguration config)
        {
            var key = config["MediatR:LicenseKey"];

            services.AddMediatR(o =>
            {
                o.LicenseKey = key;
                o.RegisterServicesFromAssemblies(
                    Assembly.Load("HelpDesk.Application"));
            });

            return services;
        }
    }
}
