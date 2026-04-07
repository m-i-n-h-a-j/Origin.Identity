using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using Origin.Identity.Application.Services.Auth;
using Origin.Identity.Infrastructure.Identity;
using Origin.Identity.Infrastructure.Persistence;
using Origin.Identity.Infrastructure.Services.Auth;

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

            services.AddDbContext<ApplicationIdentityDbContext>(options =>
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
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

            services
                .AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore().UseDbContext<ApplicationIdentityDbContext>();
                })
                .AddServer(options =>
                {
                    options.SetAuthorizationEndpointUris("/connect/authorize");
                    options.SetTokenEndpointUris("/connect/token");
                    options.SetRevocationEndpointUris("/connect/revocation");
                    options.SetEndSessionEndpointUris("/connect/logout");

                    options.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();

                    options.AllowRefreshTokenFlow();

                    options.RegisterScopes(
                        OpenIddictConstants.Scopes.OpenId,
                        OpenIddictConstants.Scopes.Profile,
                        OpenIddictConstants.Scopes.Email,
                        OpenIddictConstants.Scopes.OfflineAccess,
                        "api"
                    );

                    options.SetAccessTokenLifetime(TimeSpan.FromMinutes(10));
                    options.SetRefreshTokenLifetime(TimeSpan.FromDays(7));

                    options.DisableAccessTokenEncryption();

                    options.AddDevelopmentEncryptionCertificate();
                    options.AddDevelopmentSigningCertificate();

                    options
                        .UseAspNetCore()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableTokenEndpointPassthrough()
                        .EnableEndSessionEndpointPassthrough();
                })
                .AddValidation(options =>
                {
                    options.UseLocalServer();
                    options.UseAspNetCore();
                });

            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
