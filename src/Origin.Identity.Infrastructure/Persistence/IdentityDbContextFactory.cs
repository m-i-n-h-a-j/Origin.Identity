using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Origin.Identity.Infrastructure.Persistence
{
    public sealed class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
    {
        public IdentityDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();

            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=origin_identity;Username=postgres;Password=postgres"
            );

            return new IdentityDbContext(optionsBuilder.Options);
        }
    }
}
