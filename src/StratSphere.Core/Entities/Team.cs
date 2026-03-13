namespace StratSphere.Core.Entities;

public class Team
{
    public Guid Id { get; set; }
    public Guid LeagueId { get; set; }
    public Guid? UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }

    public League League { get; set; } = null!;
    public ApplicationUser? User { get; set; }
    public ICollection<RosterSlot> RosterSlots { get; set; } = [];
}
