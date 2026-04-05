namespace Origin.Identity.Contracts.Auth
{
    public sealed class AuthResponseDto
    {
        public string AccessToken { get; set; } = default!;

        public string RefreshToken { get; set; } = default!;

        public DateTime ExpiresAtUtc { get; set; }
    }
}
