namespace StratSphere.Core.Entities;

/// <summary>
/// MLB HallOfFame table - Hall of Fame voting records
/// </summary>
public class MlbHallOfFame
{
    public int Ballots { get; set; }
    public string? Category { get; set; }
    public int Id { get; set; }
    public int? NeededVotes { get; set; }
    public string? PlayerId { get; set; }
    public decimal? VotePct { get; set; }
    public int Votes { get; set; }
    public int Year { get; set; }
    public int? YearElected { get; set; }
    public string? YearOnBallot { get; set; }
}
