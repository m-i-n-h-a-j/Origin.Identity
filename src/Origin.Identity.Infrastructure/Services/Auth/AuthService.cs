using Microsoft.AspNetCore.Identity;
using Origin.Identity.Application.Common;
using Origin.Identity.Application.Services.Auth;
using Origin.Identity.Contracts.Auth;
using Origin.Identity.Infrastructure.Identity;

namespace Origin.Identity.Infrastructure.Services.Auth
{
    public sealed class AuthService(UserManager<ApplicationUser> userManager) : IAuthService
    {
        public async Task<Result<Guid>> RegisterAsync(RegisterRequestDto request)
        {
            var existingUser = await userManager.FindByEmailAsync(request.Email);

            if (existingUser is not null)
            {
                return Result<Guid>.Failure("A user with this email already exists.");
            }

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
            };

            var createResult = await userManager.CreateAsync(user, request.Password);

            if (!createResult.Succeeded)
            {
                var error = string.Join(
                    Environment.NewLine,
                    createResult.Errors.Select(x => x.Description)
                );

                return Result<Guid>.Failure(error);
            }

            return Result<Guid>.Success(user.Id);
        }

        public Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto request)
        {
            throw new NotImplementedException();
        }
    }
}
