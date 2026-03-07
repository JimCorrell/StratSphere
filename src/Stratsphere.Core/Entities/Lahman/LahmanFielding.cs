namespace StratSphere.Core.Entities.Lahman;

/// <summary>Read-only. Maps to lahman.fielding.</summary>
public class LahmanFielding
{
    public string PlayerId { get; set; } = string.Empty;
    public int YearId { get; set; }
    public int Stint { get; set; }
    public string? TeamId { get; set; }
    public string? LgId { get; set; }
    public string? Pos { get; set; }
    public int? G { get; set; }
    public int? GS { get; set; }
    public int? InnOuts { get; set; }
    public int? PO { get; set; }
    public int? A { get; set; }
    public int? E { get; set; }
    public int? DP { get; set; }

    public decimal? FldPct => (PO + A + E) is > 0
        ? Math.Round((decimal)((PO ?? 0) + (A ?? 0)) / ((PO ?? 0) + (A ?? 0) + (E ?? 0)), 3) : null;
}
