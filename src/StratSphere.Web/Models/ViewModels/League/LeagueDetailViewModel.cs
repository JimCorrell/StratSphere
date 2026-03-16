namespace StratSphere.Web.Models.ViewModels.League;

public class LeagueDetailViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsCommissioner { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsArchived { get; set; }
    public bool HasTeam { get; set; }
    public bool UserEmailConfirmed { get; set; }

    // Standings
    public Guid? SelectedSeasonId { get; set; }
    public int? SelectedSeasonCardYear { get; set; }
    public string? SelectedSeasonName { get; set; }
    public IEnumerable<StandingRow> Standings { get; set; } = [];

    // Commissioner reassignment
    public IEnumerable<AssignableMember> AssignableMembers { get; set; } = [];

    public IEnumerable<MemberRow> Members { get; set; } = [];
    public IEnumerable<TeamRow> Teams { get; set; } = [];
    public IEnumerable<SeasonRow> Seasons { get; set; } = [];

    public class StandingRow
    {
        public string TeamName { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Ties { get; set; }
        public decimal WinPct { get; set; }
        public int RunsScored { get; set; }
        public int RunsAllowed { get; set; }
        public int RunDiff { get; set; }
    }

    public class AssignableMember
    {
        public Guid UserId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
    }

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
        public string? ManagerName { get; set; }
        public bool IsOwner { get; set; }
        public bool IsClaimed { get; set; }
    }

    public class SeasonRow
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CardYear { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
