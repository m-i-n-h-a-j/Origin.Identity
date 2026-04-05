using Microsoft.AspNetCore.Mvc;
using Origin.Identity.Application.Services.Auth;
using Origin.Identity.Contracts.Auth;

namespace Origin.Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            var result = await authService.RegisterAsync(request);

            if (!result.IsSuccess)
            {
                return BadRequest(new { error = result.Error });
            }

            return Ok(new { userId = result.Value });
        }
    }
}
