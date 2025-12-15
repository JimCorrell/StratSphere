namespace StratSphere.Core.Entities;

/// <summary>
/// MLB Teams table - team-level historical information
/// </summary>
public class MlbTeam
{
    public int? AtBats { get; set; }
    public int? Attendance { get; set; }
    public decimal? BA { get; set; }
    public int? BPF { get; set; }
    public int? BbRef { get; set; }
    public int? BestRank { get; set; }
    public int? CaughtStealing { get; set; }
    public string? DivId { get; set; }
    public int? Doubles { get; set; }
    public int? E { get; set; }
    public int? Errs { get; set; }
    public decimal? Fpct { get; set; }
    public int? Games { get; set; }
    public int? GamesAway { get; set; }
    public int? GamesHome { get; set; }
    public int? Hits { get; set; }
    public int? HomeRuns { get; set; }
    public int Id { get; set; }
    public string? League { get; set; }
    public int? Losses { get; set; }
    public string? Name { get; set; }
    public int? Name_park_ref { get; set; }
    public decimal? OBP { get; set; }
    public decimal? OPS { get; set; }
    public int? PPF { get; set; }
    public string? Park { get; set; }
    public int? Pitchers { get; set; }
    public int? RankDiv { get; set; }
    public int? RankLg { get; set; }
    public int? Runs { get; set; }
    public decimal? SLG { get; set; }
    public int? StolenBases { get; set; }
    public int? StrikeOuts { get; set; }
    public string? TeamBbRef { get; set; }
    public string? TeamId { get; set; } // teamID from MLB
    public int? Triples { get; set; }
    public int? Wins { get; set; }
    public int Year { get; set; }
}
