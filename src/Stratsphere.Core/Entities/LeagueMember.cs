using Stratsphere.Core.Enums;

namespace Stratsphere.Core.Entities;

public class LeagueMember
{
    public Guid LeagueId { get; set; }
    public Guid UserId { get; set; }
    public LeagueRole Role { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public League League { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}
