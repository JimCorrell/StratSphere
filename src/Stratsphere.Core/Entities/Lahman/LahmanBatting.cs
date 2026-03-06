namespace Stratsphere.Core.Entities.Lahman;

/// <summary>Read-only. Maps to lahman.batting. One row per player per year per stint.</summary>
public class LahmanBatting
{
    public string PlayerId { get; set; } = string.Empty;
    public int YearId { get; set; }
    public int Stint { get; set; }
    public string? TeamId { get; set; }
    public string? LgId { get; set; }
    public int? G { get; set; }
    public int? AB { get; set; }
    public int? R { get; set; }
    public int? H { get; set; }
    public int? Doubles { get; set; }
    public int? Triples { get; set; }
    public int? HR { get; set; }
    public int? RBI { get; set; }
    public int? SB { get; set; }
    public int? CS { get; set; }
    public int? BB { get; set; }
    public int? SO { get; set; }
    public int? IBB { get; set; }
    public int? HBP { get; set; }
    public int? SH { get; set; }
    public int? SF { get; set; }
    public int? GIDP { get; set; }

    // Derived rate stats — calculated, never stored
    public decimal? BA  => AB > 0 ? Math.Round((decimal)H!.Value / AB.Value, 3) : null;
    public decimal? OBP
    {
        get
        {
            var denom = (AB ?? 0) + (BB ?? 0) + (HBP ?? 0) + (SF ?? 0);
            return denom > 0
                ? Math.Round((decimal)((H ?? 0) + (BB ?? 0) + (HBP ?? 0)) / denom, 3)
                : null;
        }
    }
    public decimal? SLG
    {
        get
        {
            if (AB is null or 0) return null;
            var tb = (H ?? 0) + (Doubles ?? 0) + (Triples ?? 0) * 2 + (HR ?? 0) * 3;
            return Math.Round((decimal)tb / AB.Value, 3);
        }
    }
    public decimal? OPS => OBP.HasValue && SLG.HasValue ? OBP + SLG : null;
}
