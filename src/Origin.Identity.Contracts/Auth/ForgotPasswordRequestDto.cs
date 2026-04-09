namespace Origin.Identity.Contracts.Auth
{
    public sealed class ForgotPasswordRequestDto
    {
        public required string Email { get; set; }
    }
}
