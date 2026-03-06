using Microsoft.AspNetCore.Identity;

namespace Stratsphere.Core.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<LeagueMember> LeagueMemberships { get; set; } = [];
    public ICollection<Team> Teams { get; set; } = [];
}
