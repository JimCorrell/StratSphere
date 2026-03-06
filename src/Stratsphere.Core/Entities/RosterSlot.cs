namespace Stratsphere.Core.Entities;

public class RosterSlot
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public Guid SeasonId { get; set; }
    public Guid CardId { get; set; }

    /// <summary>active | bench | injured</summary>
    public string SlotType { get; set; } = "active";

    /// <summary>When the card was added to this team's roster.</summary>
    public DateTimeOffset AcquiredAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>When the card was dropped/traded away. Null means currently active.</summary>
    public DateTimeOffset? DroppedAt { get; set; }

    public Team Team { get; set; } = null!;
    public Season Season { get; set; } = null!;
    public PlayerCard Card { get; set; } = null!;
}
