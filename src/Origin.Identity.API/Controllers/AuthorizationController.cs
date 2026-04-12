using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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
                var returnUrl = Uri.EscapeDataString($"{Request.Path}{Request.QueryString}");

                return Redirect($"/auth/login?returnUrl={returnUrl}");
            }

            var user = await userManager.GetUserAsync(User);

            if (user is null || !user.IsActive)
            {
                return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            var principal = await signInManager.CreateUserPrincipalAsync(user);

            principal.SetClaim(OpenIddictConstants.Claims.Subject, user.Id.ToString());

            principal.SetScopes(request.GetScopes());

            var resources = new List<string>();

            if (principal.HasScope("ragam_api"))
            {
                resources.Add("ragam_resource_server");
            }

            if (principal.HasScope("origin_api"))
            {
                resources.Add("origin_resource_server");
            }

            principal.SetResources(resources);

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        [HttpGet("~/connect/logout")]
        [HttpPost("~/connect/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            return SignOut(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties { RedirectUri = "/auth/login" }
            );
        }

        [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
        [HttpGet("~/connect/userinfo")]
        public async Task<IActionResult> UserInfo()
        {
            var subject = User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(subject))
            {
                return Forbid();
            }

            var user = await userManager.FindByIdAsync(subject);

            if (user is null)
            {
                return Forbid();
            }

            return Ok(new
            {
                sub = user.Id,
                email = user.Email,
                name = user.UserName
            });
        }
    }
}
