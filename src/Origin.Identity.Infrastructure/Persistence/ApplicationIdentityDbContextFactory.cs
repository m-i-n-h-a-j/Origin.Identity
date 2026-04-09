using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Origin.Identity.Infrastructure.Persistence
{
    public sealed class ApplicationIdentityDbContextFactory
        : IDesignTimeDbContextFactory<ApplicationIdentityDbContext>
    {
        public ApplicationIdentityDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            string? defaultConnection = configuration.GetConnectionString("DefaultConnection");
            string? neonDbPooledConnection = configuration.GetConnectionString("NeonDbPooled");
            string? neonDbDirectConnection = configuration.GetConnectionString("NeonDbDirect");

            var connectionString =
                neonDbDirectConnection
                ?? throw new InvalidOperationException(
                    "Connection string 'IdentityDatabase' was not found."
                );

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationIdentityDbContext>();

            optionsBuilder.UseNpgsql(connectionString);
            optionsBuilder.UseOpenIddict();

            return new ApplicationIdentityDbContext(optionsBuilder.Options);
        }
    }
}
