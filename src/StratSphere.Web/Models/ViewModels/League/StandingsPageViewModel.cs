namespace StratSphere.Web.Models.ViewModels.League;

public class StandingsPageViewModel
{
    public string LeagueName         { get; set; } = "";
    public string LeagueAbbreviation { get; set; } = "";
    public int?   CardYear           { get; set; }
    public bool   IsCommissioner     { get; set; }
    public string ActiveView         { get; set; } = "league";
    public IEnumerable<StandingsRow> Teams { get; set; } = [];

    public class StandingsRow
    {
        public Guid    TeamId        { get; set; }
        public string  City          { get; set; } = "";
        public string  Name          { get; set; } = "";
        public string  Abbreviation  { get; set; } = "";
        public string? Color         { get; set; }
        public string? ColorInk      { get; set; }
        public string? Monogram      { get; set; }

        public int     Wins          { get; set; }
        public int     Losses        { get; set; }
        public int     Ties          { get; set; }
        public decimal WinPct        { get; set; }
        public decimal GB            { get; set; }
        public int     RunsScored    { get; set; }
        public int     RunsAllowed   { get; set; }
        public int     RunDiff       { get; set; }
        public string? Streak        { get; set; }
        public int     HomeWins      { get; set; }
        public int     HomeLosses    { get; set; }
        public int     AwayWins      { get; set; }
        public int     AwayLosses    { get; set; }
        public int     Last10Wins    { get; set; }
        public int     Last10Losses  { get; set; }

        public string WinPctDisplay  => Wins + Losses + Ties == 0 ? "—" : WinPct.ToString(".000");
        public string GBDisplay      => GB == 0 ? "—" : (GB % 1 == 0 ? GB.ToString("0") : GB.ToString("0.0"));
        public string HomeRecord     => $"{HomeWins}-{HomeLosses}";
        public string AwayRecord     => $"{AwayWins}-{AwayLosses}";
        public string Last10Display  => $"{Last10Wins}-{Last10Losses}";
        public string RunDiffDisplay => RunDiff == 0 ? "0" : (RunDiff > 0 ? $"+{RunDiff}" : $"{RunDiff}");
    }
}
