using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Origin.Identity.Infrastructure.Identity.OpenIddict
{
    public static class OpenIddictSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var applicationManager =
                scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            await SeedApiScopeAsync(scope);
            await SeedAngularSpaApplicationAsync(applicationManager);
        }

        public static async Task SeedApiScopeAsync(IServiceScope serviceScope)
        {
            var applicationManager =
                serviceScope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            var scopeManager =
                serviceScope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

            var apiScope = await scopeManager.FindByNameAsync("origin_api");

            var apiScopeDescriptor = new OpenIddictScopeDescriptor
            {
                Name = "origin_api",
                DisplayName = "Main API Access",
            };

            apiScopeDescriptor.Resources.Add("resource_server");

            if (apiScope is null)
            {
                await scopeManager.CreateAsync(apiScopeDescriptor);

            }
            else
            {
                await scopeManager.UpdateAsync(apiScope, apiScopeDescriptor);


            }
        }

        public static async Task SeedAngularSpaApplicationAsync(
            IOpenIddictApplicationManager applicationManager
        )
        {
            const string clientId = "angular-spa";

            var existingApplication = await applicationManager.FindByClientIdAsync(clientId);

            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = clientId,
                DisplayName = "Angular SPA",

                ClientType = ClientTypes.Public,

                ConsentType = ConsentTypes.Implicit,
            };

            descriptor.RedirectUris.Add(new Uri("http://localhost:4200/auth/callback"));

            descriptor.PostLogoutRedirectUris.Add(new Uri("http://localhost:4200"));

            descriptor.Permissions.UnionWith([
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.Endpoints.EndSession,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.RefreshToken,
                Permissions.ResponseTypes.Code,
                Permissions.Prefixes.Scope + Scopes.OpenId,
                Permissions.Prefixes.Scope + Scopes.Profile,
                Permissions.Prefixes.Scope + Scopes.Email,
                Permissions.Prefixes.Scope + Scopes.OfflineAccess,
                Permissions.Prefixes.Scope + "origin_api",
            ]);

            descriptor.Requirements.Add(Requirements.Features.ProofKeyForCodeExchange);

            if (existingApplication is null)
            {
                await applicationManager.CreateAsync(descriptor);
            }
            else
            {
                await applicationManager.UpdateAsync(existingApplication, descriptor);
            }
        }
    }
}
