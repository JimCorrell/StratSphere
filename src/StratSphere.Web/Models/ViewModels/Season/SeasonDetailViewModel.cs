namespace StratSphere.Web.Models.ViewModels.Season;

public class SeasonDetailViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CardYear { get; set; }
    public string Status { get; set; } = string.Empty;
    public string LeagueName { get; set; } = string.Empty;
    public string LeagueSlug { get; set; } = string.Empty;
    public bool IsCommissioner { get; set; }
}
