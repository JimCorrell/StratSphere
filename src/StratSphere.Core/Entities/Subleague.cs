namespace StratSphere.Core.Entities;

public class Subleague : TenantEntity
{
    public string? Abbreviation { get; set; }
    public ICollection<Division> Divisions { get; set; } = new List<Division>();
    public string Name { get; set; } = string.Empty;
    public ICollection<Team> Teams { get; set; } = new List<Team>();
}
