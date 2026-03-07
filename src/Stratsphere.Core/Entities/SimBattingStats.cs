namespace StratSphere.Core.Entities;

/// <summary>
/// Accumulated batting stats for a PlayerCard within a specific league season.
/// One row per (CardId, SeasonId) — fully isolated across leagues.
/// Created with all zeros when a card is added to a roster; incremented as games are played.
/// </summary>
public class SimBattingStats
{
    public Guid Id { get; set; }
    public Guid CardId { get; set; }
    public Guid SeasonId { get; set; }
    public Guid TeamId { get; set; }  // current team (will change via trades in V2)

    public int G { get; set; }
    public int AB { get; set; }
    public int R { get; set; }
    public int H { get; set; }
    public int Doubles { get; set; }
    public int Triples { get; set; }
    public int HR { get; set; }
    public int RBI { get; set; }
    public int BB { get; set; }
    public int SO { get; set; }
    public int SB { get; set; }
    public int CS { get; set; }
    public int HBP { get; set; }
    public int SF { get; set; }

    // Derived — calculated, never stored
    public decimal? BA  => AB > 0 ? Math.Round((decimal)H / AB, 3) : null;
    public decimal? OBP => (AB + BB + HBP + SF) > 0
        ? Math.Round((decimal)(H + BB + HBP) / (AB + BB + HBP + SF), 3) : null;
    public decimal? SLG => AB > 0
        ? Math.Round((decimal)(H - Doubles - Triples - HR + Doubles * 2 + Triples * 3 + HR * 4) / AB, 3) : null;
    public decimal? OPS => OBP.HasValue && SLG.HasValue ? OBP + SLG : null;

    public PlayerCard Card { get; set; } = null!;
    public Season Season { get; set; } = null!;
    public Team Team { get; set; } = null!;
}
