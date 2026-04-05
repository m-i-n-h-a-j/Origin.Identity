using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Origin.Identity.Infrastructure.Identity;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Origin.Identity.API.Controllers
{
    [ApiController]
    public sealed class AuthorizationController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager
    ) : ControllerBase
    {
        [HttpGet("~/connect/authorize")]
        [HttpPost("~/connect/authorize")]
        public async Task<IActionResult> Authorize()
        {
            var request =
                HttpContext.GetOpenIddictServerRequest()
                ?? throw new InvalidOperationException(
                    "The OpenID Connect request cannot be retrieved."
                );

            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Challenge(
                    authenticationSchemes: [IdentityConstants.ApplicationScheme],
                    properties: new AuthenticationProperties
                    {
                        RedirectUri = Request.Path + Request.QueryString,
                    }
                );
            }

            var user = await userManager.GetUserAsync(User);

            if (user is null || !user.IsActive)
            {
                return Forbid(
                    authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]
                );
            }

            var principal = await signInManager.CreateUserPrincipalAsync(user);

            var identity = (ClaimsIdentity)principal.Identity!;

            identity.SetClaim(Claims.Subject, user.Id.ToString());

            principal.SetScopes(request.GetScopes());

            principal.SetDestinations(claim =>
                claim.Type switch
                {
                    Claims.Name => [Destinations.AccessToken, Destinations.IdentityToken],

                    Claims.Email => [Destinations.AccessToken, Destinations.IdentityToken],

                    Claims.Role => [Destinations.AccessToken],

                    _ => [Destinations.AccessToken],
                }
            );

            user.LastLoginAtUtc = DateTime.UtcNow;
            await userManager.UpdateAsync(user);

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        [HttpPost("~/connect/token")]
        public async Task<IActionResult> Exchange()
        {
            var request =
                HttpContext.GetOpenIddictServerRequest()
                ?? throw new InvalidOperationException(
                    "The OpenID Connect request cannot be retrieved."
                );

            if (request.IsAuthorizationCodeGrantType() || request.IsRefreshTokenGrantType())
            {
                var result = await HttpContext.AuthenticateAsync(
                    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                );

                var principal =
                    result.Principal
                    ?? throw new InvalidOperationException(
                        "The token principal cannot be retrieved."
                    );

                var userId = principal.GetClaim(Claims.Subject);

                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Forbid(
                        authenticationSchemes:
                        [
                            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        ]
                    );
                }

                var user = await userManager.FindByIdAsync(userId);

                if (user is null || !user.IsActive)
                {
                    return Forbid(
                        authenticationSchemes:
                        [
                            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        ],
                        properties: new AuthenticationProperties(
                            new Dictionary<string, string?>
                            {
                                [Parameters.Error] = Errors.InvalidGrant,
                                [Parameters.ErrorDescription] =
                                    "The user account is no longer valid.",
                            }
                        )
                    );
                }

                var newPrincipal = await signInManager.CreateUserPrincipalAsync(user);

                var identity = (ClaimsIdentity)newPrincipal.Identity!;

                identity.SetClaim(Claims.Subject, user.Id.ToString());

                newPrincipal.SetScopes(principal.GetScopes());

                newPrincipal.SetDestinations(claim =>
                    claim.Type switch
                    {
                        Claims.Name => [Destinations.AccessToken, Destinations.IdentityToken],

                        Claims.Email => [Destinations.AccessToken, Destinations.IdentityToken],

                        Claims.Role => [Destinations.AccessToken],

                        _ => [Destinations.AccessToken],
                    }
                );

                return SignIn(
                    newPrincipal,
                    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                );
            }

            throw new NotSupportedException("The specified grant type is not supported.");
        }
    }
}
