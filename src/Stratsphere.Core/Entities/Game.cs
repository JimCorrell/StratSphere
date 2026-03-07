using StratSphere.Core.Enums;

namespace StratSphere.Core.Entities;

public class Game
{
    public Guid Id { get; set; }
    public Guid SeasonId { get; set; }
    public Guid HomeTeamId { get; set; }
    public Guid AwayTeamId { get; set; }
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public DateOnly GameDate { get; set; }
    public GameStatus Status { get; set; } = GameStatus.Scheduled;

    public Season Season { get; set; } = null!;
    public Team HomeTeam { get; set; } = null!;
    public Team AwayTeam { get; set; } = null!;
}
