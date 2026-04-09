using Origin.Identity.Application.Common;
using Origin.Identity.Contracts.Auth;

namespace Origin.Identity.Application.Services.Auth
{
    public interface IAuthService
    {
        Task<Result<Guid>> RegisterAsync(RegisterRequestDto request);

        Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto request);

        Task<Result<string>> DeleteUserAsync(string id);
    }
}
