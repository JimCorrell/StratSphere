namespace Stratsphere.Web.Models.ViewModels.Team;

public class TeamDetailViewModel
{
    public Guid Id { get; set; }
    public string City { get; set; } = "";
    public string Name { get; set; } = "";
    public string Abbreviation { get; set; } = "";
    public string LeagueName { get; set; } = "";
    public string LeagueSlug { get; set; } = "";
    public bool IsOwner { get; set; }
}
