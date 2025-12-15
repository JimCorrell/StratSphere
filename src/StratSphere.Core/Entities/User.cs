namespace StratSphere.Core.Entities;

public class User : BaseEntity
{
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool IsAdmin { get; set; } = false;
    public DateTime? LastLoginAt { get; set; }
    public League? LastVisitedLeague { get; set; }

    // Multi-league context
    public Guid? LastVisitedLeagueId { get; set; }

    // Navigation properties
    public ICollection<LeagueMember> LeagueMemberships { get; set; } = new List<LeagueMember>();
    public string PasswordHash { get; set; } = string.Empty;
    public ICollection<Team> Teams { get; set; } = new List<Team>();
    public string Username { get; set; } = string.Empty;
}
