namespace Stratsphere.Core.Entities;

/// <summary>
/// Represents a Strat-O-Matic card: one real player in one real MLB season.
/// The card definition is shared globally across all leagues.
/// Stats are tracked separately: historical (Lahman) and simulated (per league season).
/// </summary>
public class PlayerCard
{
    public Guid Id { get; set; }

    /// <summary>Lahman playerID, e.g. "schmimi01". Soft reference to lahman.people.</summary>
    public string LahmanPlayerId { get; set; } = string.Empty;

    /// <summary>The real MLB season year this card represents, e.g. 1986.</summary>
    public int CardYear { get; set; }

    public string Position { get; set; } = string.Empty;

    public ICollection<RosterSlot> RosterSlots { get; set; } = [];
    public ICollection<SimBattingStats> SimBattingStats { get; set; } = [];
    public ICollection<SimPitchingStats> SimPitchingStats { get; set; } = [];
}
