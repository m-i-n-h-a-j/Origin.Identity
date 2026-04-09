namespace Origin.Identity.Contracts.Auth
{
    public sealed class ResetPasswordRequestDto
    {
        public required string Email { get; set; }

        public required string Token { get; set; }

        public required string NewPassword { get; set; }
    }
}
