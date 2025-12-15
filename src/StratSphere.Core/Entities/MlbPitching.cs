namespace StratSphere.Core.Entities;

/// <summary>
/// MLB Pitching table - annual pitching statistics
/// </summary>
public class MlbPitching
{
    public int? AREA { get; set; }
    public int? BB { get; set; }
    public int? BK { get; set; }
    public int? CG { get; set; }
    public int? ER { get; set; }
    public decimal? ERA { get; set; }
    public int? G { get; set; }
    public int? GS { get; set; }
    public int? H { get; set; }
    public int? HBP { get; set; }
    public int? HR { get; set; }
    public int? IBB { get; set; }
    public int? IPouts { get; set; }
    public int Id { get; set; }
    public int? K { get; set; }
    public int? L { get; set; }
    public string? LgId { get; set; }
    public string? PlayerId { get; set; }
    public int? SHO { get; set; }
    public int? SV { get; set; }
    public int Stint { get; set; }
    public string? TeamId { get; set; }
    public int? W { get; set; }
    public decimal? WHIP { get; set; }
    public int? WP { get; set; }
    public int Year { get; set; }
}
