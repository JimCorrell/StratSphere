namespace StratSphere.Core.Entities;

/// <summary>
/// MLB Batting table - annual batting statistics
/// </summary>
public class MlbBatting
{
    public int? AB { get; set; }
    public decimal? BA { get; set; }
    public int? BB { get; set; }
    public int? CS { get; set; }
    public int? Doubles { get; set; }
    public int? GIDP { get; set; }
    public int? Games { get; set; }
    public int? HBP { get; set; }
    public int? HR { get; set; }
    public int? Hits { get; set; }
    public int? IBB { get; set; }
    public int Id { get; set; }
    public string? LgId { get; set; }
    public decimal? OBP { get; set; }
    public decimal? OPS { get; set; }
    public string? PlayerId { get; set; }
    public int? RBIs { get; set; }
    public int? Runs { get; set; }
    public int? SB { get; set; }
    public int? SF { get; set; }
    public int? SH { get; set; }
    public decimal? SLG { get; set; }
    public int? SO { get; set; }
    public int Stint { get; set; }
    public string? TeamId { get; set; }
    public int? Triples { get; set; }
    public int Year { get; set; }
}
