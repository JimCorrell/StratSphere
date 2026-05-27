namespace StratSphere.Web.Models.ViewModels.Import;

public class SomMapTeamsViewModel
{
    public string LeagueAbbreviation { get; set; } = "";
    public string LeagueName         { get; set; } = "";
    public string SeasonName         { get; set; } = "";
    public string TempFileKey        { get; set; } = "";

    /// <summary>SOM team abbreviations in index order (from .H01 header).</summary>
    public string[] SomTeams { get; set; } = [];

    /// <summary>StratSphere teams in this season available for mapping.</summary>
    public List<TeamOption> StratSphereTeams { get; set; } = [];

    /// <summary>Pre-populated mapping: SOM abbr → StratSphere team ID (if auto-matched).</summary>
    public Dictionary<string, Guid> Suggestions { get; set; } = [];
}

public class TeamOption
{
    public Guid   Id           { get; set; }
    public string Abbreviation { get; set; } = "";
    public string DisplayName  { get; set; } = "";
}
