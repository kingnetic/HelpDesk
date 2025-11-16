using HelpDesk.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HelpDesk.Infrastructure.Auth
{
    public static class JwtAuthenticationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration config)
        {
            var section = config.GetSection("JwtSettings");

            var secretKey = section["Key"];
            var issuer = section["Issuer"];
            var audience = section["Audience"];

            if (string.IsNullOrWhiteSpace(secretKey))
                throw new Exception("JwtSettings:Key is missing.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = key,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        NameClaimType = ClaimTypes.Email,
                        RoleClaimType = ClaimTypes.Role
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async context =>
                        {
                            try
                            {
                                var jtiString = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                                if (!Guid.TryParse(jtiString, out var jti))
                                {
                                    context.Fail("Invalid JTI");
                                    return;
                                }

                                var db = context.HttpContext.RequestServices.GetRequiredService<HelpDeskDbContext>();

                                var session = await db.UserSessions
                                    .FirstOrDefaultAsync(s => s.JwtId == jti);

                                if (session == null)
                                {
                                    context.Fail("Session not found.");
                                    return;
                                }

                                if (!session.IsActiveSession)
                                {
                                    context.Fail("Session is closed.");
                                    return;
                                }

                                if (session.TokenExpiresAt < DateTime.UtcNow)
                                {
                                    context.Fail("Token expired per session record.");
                                    return;
                                }
                            }
                            catch
                            {
                                context.Fail("Token validation error.");
                            }
                        }
                    };
                });

            return services;
        }
    }
}
