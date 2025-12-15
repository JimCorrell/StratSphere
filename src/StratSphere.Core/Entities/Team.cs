namespace StratSphere.Core.Entities;

public class Team : TenantEntity
{
    public string Abbreviation { get; set; } = string.Empty;
    public ICollection<GameResult> AwayGames { get; set; } = new List<GameResult>();
    public string? City { get; set; }
    public string? Conference { get; set; }
    public Division? Division { get; set; }
    public Guid? DivisionId { get; set; }
    public string? DivisionName { get; set; }
    public ICollection<DraftPick> DraftPicks { get; set; } = new List<DraftPick>();
    public ICollection<GameResult> HomeGames { get; set; } = new List<GameResult>();
    public string? LogoUrl { get; set; }
    public string Name { get; set; } = string.Empty;
    public User Owner { get; set; } = null!;

    // Owner
    public Guid OwnerId { get; set; }

    // Navigation properties
    public ICollection<RosterEntry> Roster { get; set; } = new List<RosterEntry>();
    public ICollection<StandingsEntry> Standings { get; set; } = new List<StandingsEntry>();
    public Subleague? Subleague { get; set; }

    // League structure
    public Guid? SubleagueId { get; set; }
}
