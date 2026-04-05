using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Origin.Identity.Infrastructure.Identity;

namespace Origin.Identity.Infrastructure.Persistence
{
    public sealed class ApplicationIdentityDbContext(
        DbContextOptions<ApplicationIdentityDbContext> options
    )
        : IdentityDbContext<
            ApplicationUser,
            ApplicationRole,
            Guid,
            IdentityUserClaim<Guid>,
            IdentityUserRole<Guid>,
            IdentityUserLogin<Guid>,
            IdentityRoleClaim<Guid>,
            IdentityUserToken<Guid>,
            IdentityUserPasskey<Guid>
        >(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema("identity");

            builder.UseOpenIddict();

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(x => x.LastName).IsRequired().HasMaxLength(100);
            });
        }
    }
}
