using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Origin.Identity.Infrastructure.Persistence
{
    public sealed class ApplicationIdentityDbContextFactory
        : IDesignTimeDbContextFactory<ApplicationIdentityDbContext>
    {
        public ApplicationIdentityDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationIdentityDbContext>();

            optionsBuilder.UseNpgsql(
                "Host=10.1.248.10;Port=5432;Database=origin_identity;Username=postgres;Password=pgsu123#"
            );

            return new ApplicationIdentityDbContext(optionsBuilder.Options);
        }
    }
}
