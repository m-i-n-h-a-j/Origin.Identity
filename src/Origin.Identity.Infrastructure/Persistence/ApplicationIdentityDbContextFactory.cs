using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Origin.Identity.Infrastructure.Persistence
{
    public sealed class ApplicationIdentityDbContextFactory : IDesignTimeDbContextFactory<ApplicationIdentityDbContext>
    {
        public ApplicationIdentityDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationIdentityDbContext>();

            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=origin_identity;Username=postgres;Password=postgres"
            );

            return new ApplicationIdentityDbContext(optionsBuilder.Options);
        }
    }
}
