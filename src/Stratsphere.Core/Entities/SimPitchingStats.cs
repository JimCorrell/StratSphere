namespace Stratsphere.Core.Entities;

/// <summary>
/// Accumulated pitching stats for a PlayerCard within a specific league season.
/// One row per (CardId, SeasonId) — fully isolated across leagues.
/// </summary>
public class SimPitchingStats
{
    public Guid Id { get; set; }
    public Guid CardId { get; set; }
    public Guid SeasonId { get; set; }
    public Guid TeamId { get; set; }

    public int W { get; set; }
    public int L { get; set; }
    public int G { get; set; }
    public int GS { get; set; }
    public int SV { get; set; }

    /// <summary>Outs recorded (IP * 3). Divide by 3.0 for display.</summary>
    public int IPOuts { get; set; }

    public int H { get; set; }
    public int ER { get; set; }
    public int HR { get; set; }
    public int BB { get; set; }
    public int SO { get; set; }

    // Derived
    public decimal? IP  => IPOuts > 0 ? Math.Round(IPOuts / 3m, 1) : null;
    public decimal? ERA => IPOuts > 0 ? Math.Round((decimal)ER / IPOuts * 27, 2) : null;
    public decimal? WHIP => IPOuts > 0 ? Math.Round((decimal)(BB + H) / IPOuts * 3, 3) : null;
    public decimal? K9  => IPOuts > 0 ? Math.Round((decimal)SO / IPOuts * 27, 2) : null;

    public PlayerCard Card { get; set; } = null!;
    public Season Season { get; set; } = null!;
    public Team Team { get; set; } = null!;
}
