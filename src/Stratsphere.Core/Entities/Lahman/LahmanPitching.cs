namespace Stratsphere.Core.Entities.Lahman;

/// <summary>Read-only. Maps to lahman.pitching. One row per player per year per stint.</summary>
public class LahmanPitching
{
    public string PlayerId { get; set; } = string.Empty;
    public int YearId { get; set; }
    public int Stint { get; set; }
    public string? TeamId { get; set; }
    public string? LgId { get; set; }
    public int? W { get; set; }
    public int? L { get; set; }
    public int? G { get; set; }
    public int? GS { get; set; }
    public int? CG { get; set; }
    public int? SHO { get; set; }
    public int? SV { get; set; }

    /// <summary>Outs recorded (IP * 3).</summary>
    public int? IPOuts { get; set; }

    public int? H { get; set; }
    public int? ER { get; set; }
    public int? HR { get; set; }
    public int? BB { get; set; }
    public int? SO { get; set; }
    public decimal? ERAStored { get; set; }

    // Derived
    public decimal? IP   => IPOuts.HasValue ? Math.Round(IPOuts.Value / 3m, 1) : null;
    public decimal? ERA  => IPOuts is > 0 ? Math.Round((decimal)ER!.Value / IPOuts.Value * 27, 2) : null;
    public decimal? WHIP => IPOuts is > 0 ? Math.Round((decimal)((BB ?? 0) + (H ?? 0)) / IPOuts.Value * 3, 3) : null;
    public decimal? K9   => IPOuts is > 0 ? Math.Round((decimal)(SO ?? 0) / IPOuts.Value * 27, 2) : null;
}
