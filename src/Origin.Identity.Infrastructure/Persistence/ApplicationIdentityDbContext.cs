using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;
using Origin.Identity.Infrastructure.Identity;

namespace Origin.Identity.Infrastructure.Persistence
{
    public sealed class ApplicationIdentityDbContext(
        DbContextOptions<ApplicationIdentityDbContext> options
    ) : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema("identity");

            #region Identity Tables
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Users");

                entity.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(x => x.LastName).IsRequired().HasMaxLength(100);
            });

            builder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable("Roles");
            });
            builder.Entity<IdentityUserRole<Guid>>(entity =>
            {
                entity.ToTable("UserRoles");
            });
            builder.Entity<IdentityUserClaim<Guid>>(entity =>
            {
                entity.ToTable("UserClaims");
            });
            builder.Entity<IdentityUserLogin<Guid>>(entity =>
            {
                entity.ToTable("UserLogins");
            });
            builder.Entity<IdentityUserToken<Guid>>(entity =>
            {
                entity.ToTable("UserTokens");
            });
            builder.Entity<IdentityRoleClaim<Guid>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });
            #endregion

            #region OpenIddict Tables
            builder.Entity<OpenIddictEntityFrameworkCoreApplication<Guid>>(entity =>
            {
                entity.ToTable("Applications");
            });

            builder.Entity<OpenIddictEntityFrameworkCoreAuthorization<Guid>>(entity =>
            {
                entity.ToTable("Authorizations");
            });

            builder.Entity<OpenIddictEntityFrameworkCoreScope<Guid>>(entity =>
            {
                entity.ToTable("Scopes");
            });

            builder.Entity<OpenIddictEntityFrameworkCoreToken<Guid>>(entity =>
            {
                entity.ToTable("Tokens");
            });
            #endregion
        }
    }
}
