namespace Origin.Identity.Contracts.Auth
{
    public sealed class RegisterRequestDto
    {
        public string FirstName { get; set; } = default!;

        public string LastName { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string Password { get; set; } = default!;
    }
}
