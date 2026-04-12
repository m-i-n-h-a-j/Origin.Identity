using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
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
            IConfiguration configuration,
            bool isDevelopment
        )
        {
            string? defaultConnection = configuration.GetConnectionString("DefaultConnection");
            string? neonDbPooledConnection = configuration.GetConnectionString("NeonDbPooled");
            string? neonDbDirectConnection = configuration.GetConnectionString("NeonDbDirect");

            services.AddDbContext<ApplicationIdentityDbContext>(options =>
            {
                options.UseNpgsql(
                    neonDbPooledConnection,
                    npgsql =>
                    {
                        npgsql.CommandTimeout(30);
                        npgsql.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(2),
                            errorCodesToAdd: null
                        );
                    }
                );

                options.UseOpenIddict();
            });

            services.AddDataProtection().PersistKeysToDbContext<ApplicationIdentityDbContext>();

            services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.User.RequireUniqueEmail = true;

                    options.Password.RequiredLength = 8;
                    options.Password.RequireDigit = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = false;

                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;
                })
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

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
                    options.SetUserInfoEndpointUris("/connect/userinfo");
                    options.SetIntrospectionEndpointUris("/connect/introspect");

                    options.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();
                    options.AllowRefreshTokenFlow();
                    options.UseReferenceRefreshTokens();

                    options.RegisterScopes(
                        OpenIddictConstants.Scopes.OpenId,
                        OpenIddictConstants.Scopes.Profile,
                        OpenIddictConstants.Scopes.Email,
                        OpenIddictConstants.Scopes.OfflineAccess,
                        "origin_api",
                        "ragam_api"
                    );

                    options.SetAccessTokenLifetime(TimeSpan.FromSeconds(5));
                    options.SetRefreshTokenLifetime(TimeSpan.FromDays(90));

                    if (isDevelopment)
                    {
                        options.DisableAccessTokenEncryption();
                        options.AddDevelopmentEncryptionCertificate();
                        options.AddDevelopmentSigningCertificate();
                    }
                    else
                    {
                        var signingBytes = Convert.FromBase64String(
                            File.ReadAllText(configuration["OpenIddict:SigningCertificate:Path"]!)
                        );

                        var signingCertificate = X509CertificateLoader.LoadPkcs12(
                            signingBytes,
                            configuration["OpenIddict:SigningCertificate:Password"]
                        );

                        var encryptionBytes = Convert.FromBase64String(
                            File.ReadAllText(
                                configuration["OpenIddict:EncryptionCertificate:Path"]!
                            )
                        );

                        var encryptionCertificate = X509CertificateLoader.LoadPkcs12(
                            encryptionBytes,
                            configuration["OpenIddict:EncryptionCertificate:Password"]
                        );

                        options.AddSigningCertificate(signingCertificate);
                        options.AddEncryptionCertificate(encryptionCertificate);
                    }

                    options
                        .UseAspNetCore()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableEndSessionEndpointPassthrough()
                        .EnableUserInfoEndpointPassthrough();
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
