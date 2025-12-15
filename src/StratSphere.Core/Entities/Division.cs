namespace StratSphere.Core.Entities;

public class Division : TenantEntity
{
    public string? Abbreviation { get; set; }
    public string Name { get; set; } = string.Empty;
    public Subleague Subleague { get; set; } = null!;
    public Guid SubleagueId { get; set; }
    public ICollection<Team> Teams { get; set; } = new List<Team>();
}
