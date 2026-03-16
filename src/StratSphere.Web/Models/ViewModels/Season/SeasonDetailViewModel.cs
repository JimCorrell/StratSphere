namespace StratSphere.Web.Models.ViewModels.Season;

public class SeasonDetailViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CardYear { get; set; }
    public string Status { get; set; } = string.Empty;
    public string LeagueName { get; set; } = string.Empty;
    public string LeagueAbbreviation { get; set; } = string.Empty;
    public bool IsCommissioner { get; set; }
    public IEnumerable<TeamRosterRow> Teams { get; set; } = [];

    public class TeamRosterRow
    {
        public Guid TeamId { get; set; }
        public string City { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public string? ManagerName { get; set; }
        public bool IsClaimed { get; set; }
        public int RosterCount { get; set; }
    }
}
