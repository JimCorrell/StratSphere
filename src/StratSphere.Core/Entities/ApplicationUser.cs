using Microsoft.AspNetCore.Identity;

namespace StratSphere.Core.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsAdmin { get; set; } = false;

    public ICollection<LeagueMember> LeagueMemberships { get; set; } = [];
    public ICollection<Team> Teams { get; set; } = [];
}
