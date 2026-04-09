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

        [HttpPost("login")]
        public async Task<IResult> Login(LoginRequestDto request)
        {
            var result = await authService.LoginAsync(request);

            if (!result.IsSuccess)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(result.Value);
        }

        [HttpDelete("users/{id}")]
        public async Task<IResult> DeleteUser(string id)
        {
            var result = await authService.DeleteUserAsync(id);

            if (!result.IsSuccess)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.NoContent();
        }

        [HttpPost("forgot")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDto request)
        {
            var result = await authService.ForgotPasswordAsync(request);

            return Ok(new { message = "Password reset link has been sent to email." });
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto request)
        {
            var result = await authService.ResetPasswordAsync(request);

            if (!result.IsSuccess)
            {
                return BadRequest(new { error = result.Error });
            }

            return Ok(new { message = "Password has been reset successfully." });
        }
    }
}
