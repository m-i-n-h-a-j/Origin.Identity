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

            var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

            var apiScope = await scopeManager.FindByNameAsync("api");

            var apiScopeDescriptor = new OpenIddictScopeDescriptor
            {
                Name = "api",
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

            var postmanApplication = await applicationManager.FindByClientIdAsync("postman");

            var postmanDescriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = "postman",
                DisplayName = "Postman",

                ClientType = ClientTypes.Public,
                ConsentType = ConsentTypes.Explicit,
            };

            postmanDescriptor.RedirectUris.Add(new Uri("https://oauth.pstmn.io/v1/callback"));

            postmanDescriptor.Permissions.UnionWith([
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.RefreshToken,
                Permissions.ResponseTypes.Code,
                Permissions.Prefixes.Scope + Scopes.OpenId,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Email,
                Permissions.Prefixes.Scope + Scopes.OfflineAccess,
                Permissions.Prefixes.Scope + "api",
            ]);

            postmanDescriptor.Requirements.Add(Requirements.Features.ProofKeyForCodeExchange);

            if (postmanApplication is null)
            {
                await applicationManager.CreateAsync(postmanDescriptor);
            }
            else
            {
                await applicationManager.UpdateAsync(postmanApplication, postmanDescriptor);
            }
        }
    }
}
