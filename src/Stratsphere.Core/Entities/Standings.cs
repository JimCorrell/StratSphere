namespace Stratsphere.Core.Entities;

public class Standings
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public Guid SeasonId { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Ties { get; set; }
    public int RunsScored { get; set; }
    public int RunsAllowed { get; set; }
    public string? Streak { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    // Derived
    public int GamesPlayed => Wins + Losses + Ties;
    public decimal WinPct => GamesPlayed > 0 ? Math.Round((decimal)(Wins + Ties * 0.5m) / GamesPlayed, 3) : 0;
    public int RunDiff => RunsScored - RunsAllowed;

    public Team Team { get; set; } = null!;
    public Season Season { get; set; } = null!;
}
