using Stratsphere.Core.Enums;

namespace Stratsphere.Core.Entities;

public class Season
{
    public Guid Id { get; set; }
    public Guid LeagueId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CardYear { get; set; }
    public SeasonStatus Status { get; set; } = SeasonStatus.Setup;
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public League League { get; set; } = null!;
    public ICollection<Team> Teams { get; set; } = [];
    public ICollection<Game> Games { get; set; } = [];
    public ICollection<Standings> Standings { get; set; } = [];
    public ICollection<RosterSlot> RosterSlots { get; set; } = [];
}
