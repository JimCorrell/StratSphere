namespace Stratsphere.Core.Entities;

public class RosterSlot
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public Guid SeasonId { get; set; }
    public Guid CardId { get; set; }

    /// <summary>active | bench | injured</summary>
    public string SlotType { get; set; } = "active";

    public Team Team { get; set; } = null!;
    public Season Season { get; set; } = null!;
    public PlayerCard Card { get; set; } = null!;
}
