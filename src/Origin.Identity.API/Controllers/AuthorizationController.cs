using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Origin.Identity.Infrastructure.Identity;

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

            if (User.Identity is not { IsAuthenticated: true })
            {
                return Challenge(
                    new AuthenticationProperties
                    {
                        RedirectUri = Request.Path + Request.QueryString,
                    },
                    IdentityConstants.ApplicationScheme
                );
            }

            var user = await userManager.GetUserAsync(User);

            if (user is null || !user.IsActive)
            {
                return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            var principal = await signInManager.CreateUserPrincipalAsync(user);

            principal.SetClaim(OpenIddictConstants.Claims.Subject, user.Id.ToString());

            principal.SetScopes(request.GetScopes());

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
    }
}
