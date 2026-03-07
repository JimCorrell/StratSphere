namespace StratSphere.Web.Models.ViewModels.Roster;

public class AddPlayerViewModel
{
    public Guid TeamId { get; set; }
    public Guid SeasonId { get; set; }
    public string LahmanPlayerId { get; set; } = "";
    public int CardYear { get; set; }
    public string Position { get; set; } = "";
}
