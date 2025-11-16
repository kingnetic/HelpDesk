using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace HelpDesk.Infrastructure.Observability
{
    public static class SerilogElasticExtension
    {
        public static void AddSerilogElastic(this IServiceCollection services, IConfiguration configuracion)
        {
            var elasticUri = configuracion["ElasticConfiguration:Uri"];

            if (string.IsNullOrEmpty(elasticUri))
                throw new ArgumentNullException("URi Elastic is required.");

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .Enrich.FromLogContext()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
                {
                    IndexFormat = $"helpdesk-api-logs-{DateTime.UtcNow:yyyy-MM}",
                    AutoRegisterTemplate = true
                })
                .Enrich.WithProperty("Application", "HelpDesk.API")
                .CreateLogger();

            services.AddLogging(logBuilder =>
            {
                logBuilder.ClearProviders();
                logBuilder.AddSerilog(dispose: true);
            });
        }
    }
}
