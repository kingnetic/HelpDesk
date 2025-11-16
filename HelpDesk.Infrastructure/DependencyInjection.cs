using HelpDesk.Application.Interfaces;
using HelpDesk.Infrastructure.Auth;
using HelpDesk.Infrastructure.Persistence;
using HelpDesk.Infrastructure.Repositories;
using HelpDesk.Infrastructure.Services;
using HelpDesk.Infrastructure.Services.Email;
using HelpDesk.Infrastructure.Services.Queries;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HelpDesk.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. DbContext
            services.AddDbContext<HelpDeskDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("HelpDeskConnection")));

            // 2. Identity (clean)
            services.AddIdentityServices();

            services.AddScoped<IAuthService, AuthService>();

            // 3. JWT
            services.AddJwtAuthentication(configuration);

            // 4. Repositories / UoW
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            
            // IHttpContextAccessor
            services.AddHttpContextAccessor();

            // Session & Auth services
            services.AddScoped<IUserSessionService, UserSessionService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITicketQueryService, TicketQueryService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuditService, AuditService>();


            // 6. Email (MailerSend SMTP)
            services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
            services.AddScoped<IEmailSender, SmtpEmailSender>();

            // 7. Mapster
            TypeAdapterConfig.GlobalSettings.Scan(Assembly.Load("HelpDesk.Application"));

            return services;
        }
    }
}
