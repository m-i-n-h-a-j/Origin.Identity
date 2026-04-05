using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Origin.Identity.Infrastructure.Identity;

namespace Origin.Identity.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseNpgsql(connectionString);

                options.UseOpenIddict();
            });

            services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.User.RequireUniqueEmail = true;

                    options.Password.RequiredLength = 8;
                    options.Password.RequireDigit = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            services
                .AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore().UseDbContext<IdentityDbContext>();
                })
                .AddServer(options =>
                {
                    options.SetTokenEndpointUris("/connect/token");

                    options.AllowPasswordFlow();
                    options.AllowRefreshTokenFlow();

                    options.AcceptAnonymousClients();

                    options.RegisterScopes("api");

                    options.AddDevelopmentEncryptionCertificate();
                    options.AddDevelopmentSigningCertificate();

                    options.UseAspNetCore().EnableTokenEndpointPassthrough();
                })
                .AddValidation(options =>
                {
                    options.UseLocalServer();
                    options.UseAspNetCore();
                });

            return services;
        }
    }
}
