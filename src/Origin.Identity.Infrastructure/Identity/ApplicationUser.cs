using Microsoft.AspNetCore.Identity;

namespace Origin.Identity.Infrastructure.Identity
{
    public sealed class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = default!;

        public string LastName { get; set; } = default!;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAtUtc { get; set; }
    }
}
