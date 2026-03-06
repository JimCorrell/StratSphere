namespace Stratsphere.Web.Models.ViewModels.League;

public class LeagueDetailViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsCommissioner { get; set; }
    public bool HasTeam { get; set; }
    public IEnumerable<MemberRow> Members { get; set; } = [];
    public IEnumerable<TeamRow> Teams { get; set; } = [];

    public class MemberRow
    {
        public string DisplayName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class TeamRow
    {
        public Guid Id { get; set; }
        public string City { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public bool IsOwner { get; set; }
    }
}
