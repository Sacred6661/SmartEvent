using Microsoft.AspNetCore.Identity;

namespace IdentityService.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string? GoogleId { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
