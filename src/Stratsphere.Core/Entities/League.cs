namespace StratSphere.Core.Entities;

public class League
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public Guid CommissionerId { get; set; }
    public string Status { get; set; } = "setup";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser Commissioner { get; set; } = null!;
    public ICollection<LeagueMember> Members { get; set; } = [];
    public ICollection<Season> Seasons { get; set; } = [];
    public ICollection<Team> Teams { get; set; } = [];
}
