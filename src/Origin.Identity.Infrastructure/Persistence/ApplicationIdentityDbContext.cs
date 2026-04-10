using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Origin.Identity.Infrastructure.Identity;

namespace Origin.Identity.Infrastructure.Persistence
{
    public sealed class ApplicationIdentityDbContext(
        DbContextOptions<ApplicationIdentityDbContext> options
    )
        : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options),
            IDataProtectionKeyContext
    {
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema("identity");

            builder.UseOpenIddict();

            builder.Entity<DataProtectionKey>(entity =>
            {
                entity.ToTable("DataProtectionKeys");
            });

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
        }
    }
}
