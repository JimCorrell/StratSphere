namespace StratSphere.Core.Entities;

/// <summary>
/// MLB Fielding table - annual fielding statistics
/// </summary>
public class MlbFielding
{
    public int? A { get; set; }
    public int? DP { get; set; }
    public int? E { get; set; }
    public decimal? Fpct { get; set; }
    public int? GS { get; set; }
    public int? Games { get; set; }
    public int Id { get; set; }
    public int? InnOuts { get; set; }
    public int? Lg { get; set; }
    public int? PO { get; set; }
    public string? PlayerId { get; set; }
    public string? Pos { get; set; }
    public int Stint { get; set; }
    public int? Tm { get; set; }
    public int Year { get; set; }
}
