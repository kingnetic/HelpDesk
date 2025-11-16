using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace HelpDesk.Infrastructure.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<HelpDeskDbContext>
    {
        public HelpDeskDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../HelpDesk.API"));

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var connectionString = config.GetConnectionString("HelpDeskConnection");

            var builder = new DbContextOptionsBuilder<HelpDeskDbContext>();
            builder.UseSqlServer(connectionString);

            return new HelpDeskDbContext(builder.Options);
        }
    }
}
