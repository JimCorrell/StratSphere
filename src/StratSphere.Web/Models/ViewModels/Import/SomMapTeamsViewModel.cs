namespace StratSphere.Web.Models.ViewModels.Import;

public class SomMapTeamsViewModel
{
    public string LeagueAbbreviation { get; set; } = "";
    public string LeagueName         { get; set; } = "";
    public string SeasonName         { get; set; } = "";
    public string TempFileKey        { get; set; } = "";

    /// <summary>SOM team abbreviations in index order (from .H01 header).</summary>
    public string[] SomTeams { get; set; } = [];

    /// <summary>StratSphere teams in this league available for mapping.</summary>
    public List<TeamOption> StratSphereTeams { get; set; } = [];

    /// <summary>Pre-populated mapping: SOM abbr → StratSphere team ID (auto-matched by abbr).</summary>
    public Dictionary<string, Guid> Suggestions { get; set; } = [];

    /// <summary>Mapping preserved from a previous Run attempt (takes precedence over Suggestions).</summary>
    public Dictionary<string, string> SavedTeamMap { get; set; } = [];

    // ── Export date / staleness ───────────────────────────────────────────────

    public DateTime? ExportDate     { get; set; }
    public DateTime? LastImportDate { get; set; }

    /// <summary>True when this archive's export date is older than the most recent import for this season.</summary>
    public bool OlderExportWarning { get; set; }

    // ── Validation ────────────────────────────────────────────────────────────

    /// <summary>Hard errors that block the import (e.g. duplicate team mapping).</summary>
    public List<string> ValidationErrors { get; set; } = [];

    /// <summary>Soft warnings the commissioner can override.</summary>
    public List<string> ValidationWarnings { get; set; } = [];

    /// <summary>Set to true when the commissioner has acknowledged all warnings.</summary>
    public bool OverrideWarnings { get; set; }
}

public class TeamOption
{
    public Guid   Id           { get; set; }
    public string Abbreviation { get; set; } = "";
    public string DisplayName  { get; set; } = "";
}
