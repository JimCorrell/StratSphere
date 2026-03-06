namespace Stratsphere.Web.Models.ViewModels.League;

public class LeagueIndexViewModel
{
    public IEnumerable<LeagueSummary> Leagues { get; set; } = [];

    public class LeagueSummary
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int MemberCount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
