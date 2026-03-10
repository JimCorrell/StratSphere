namespace StratSphere.Web.Models.ViewModels.Team;

public class TeamDetailViewModel
{
    public Guid Id { get; set; }
    public string City { get; set; } = "";
    public string Name { get; set; } = "";
    public string Abbreviation { get; set; } = "";
    public string? OwnerName { get; set; }
    public string LeagueName { get; set; } = "";
    public string LeagueSlug { get; set; } = "";
    public bool IsOwner { get; set; }
    public bool IsClaimed { get; set; }
    public bool CanManage { get; set; }

    public IEnumerable<SeasonOption> Seasons { get; set; } = [];
    public SeasonOption? SelectedSeason { get; set; }
    public IEnumerable<RosterRow> Roster { get; set; } = [];

    public class SeasonOption
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public int CardYear { get; set; }
    }

    public class RosterRow
    {
        public Guid SlotId { get; set; }
        public string PlayerName { get; set; } = "";
        public string Position { get; set; } = "";
        public int CardYear { get; set; }
        public string StatLine { get; set; } = "";
    }
}
