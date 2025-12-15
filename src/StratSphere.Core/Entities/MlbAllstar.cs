namespace StratSphere.Core.Entities;

/// <summary>
/// MLB AllstarFull table - All-Star game appearances
/// </summary>
public class MlbAllstar
{
    public string? GP { get; set; }
    public string? GameId { get; set; }
    public string? GameNum { get; set; }
    public int Id { get; set; }
    public string? LgId { get; set; }
    public string? PlayerId { get; set; }
    public string? StartingPos { get; set; }
    public string? TeamId { get; set; }
    public int Year { get; set; }
}
